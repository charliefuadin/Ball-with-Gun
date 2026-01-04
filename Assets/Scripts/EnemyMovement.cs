using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum EnemyStates
{
    Patrol,
    Attack,
    Knockback
}

public class EnemyMovement : MonoBehaviour
{
    public EnemyStates currentState;
    public EnemyStates previousState;

    private Rigidbody2D rb;
    public EnemyMovement mainController;

    public float circleRadius;
    public LayerMask groundLayer;
    public Transform groundPoint;
    private bool checkGround;
    public LayerMask wallLayer;
    public Transform wallPoint;
    private bool checkWall;

    private int patrolDirection = 1;
    public float moveSpeed;
    [HideInInspector ]public float knockbackDuration;

    private bool isGrounded = true;
    public float castDistance;
    public Vector2 boxSize;

    private Transform player;
    private bool canSee = false;
    //private bool alreadySaw = false;
    public LayerMask playerLayer;
    public Vector2 seeSize;
    public float runUpDistance;

    private Vector2 direction;
    public float jumpHeight;
    public float jumpSpeed;
    public float gravityMultiplier;
    private float gravityScale;

    public bool isJumpEnemy = false;

    public string[] collisionIgnores;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        gravityScale = rb.gravityScale;
    }

    private void FixedUpdate()
    {
        EnvironmentCheck();
        GravityMultiply();
        UpdateState();
    }

    private void EnvironmentCheck()
    {
        direction = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y);
        checkGround = Physics2D.OverlapCircle(groundPoint.position, circleRadius, groundLayer);
        checkWall = Physics2D.OverlapCircle(wallPoint.position, circleRadius, wallLayer);
        isGrounded = Physics2D.BoxCast(new Vector2(transform.position.x, transform.position.y), boxSize, 0, -transform.up, castDistance, groundLayer);
        canSee = Physics2D.OverlapBox(transform.position, seeSize, 0, playerLayer);
    }

    private void UpdateState()
    {
        switch (currentState)
        {
            case EnemyStates.Patrol:
                if (canSee || mainController.canSee)
                {
                    currentState = EnemyStates.Attack;
                }
                if (isJumpEnemy && isGrounded)
                {
                    Patrol();
                }
                break;
            case EnemyStates.Attack: //Checks which type of enemy also
                if(isJumpEnemy && isGrounded)
                {
                    JumpAttack();
                }
                else
                {
                    MoveToPlayer();
                }
                    break;
            case EnemyStates.Knockback:
                HandleKnockback();
                break;
        }
    }

    public void HandleKnockback()
    {
        knockbackDuration -= Time.fixedDeltaTime;
        if(knockbackDuration <= 0)
        {
            //Makes sure the ai goes back to what is was previously doing
            currentState = previousState;
        }
    }

    private void Patrol()
    {
        //Checks when to reverse
        if(!checkGround || checkWall)
        {
            transform.Rotate(0, 180, 0);
            patrolDirection *= -1;
        }
        rb.velocity = new Vector2(patrolDirection * moveSpeed, rb.velocity.y);
    }

    private void JumpAttack()
    {
        float distanceFromPlayer = player.position.x - transform.position.x;
        if (Mathf.Abs(distanceFromPlayer) <= runUpDistance)
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2((distanceFromPlayer) *jumpSpeed, jumpHeight), ForceMode2D.Impulse);
        }
        else
        {
            MoveToPlayer();
        }
    }


    private void MoveToPlayer()
    {
        direction.Normalize();
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
    }

    private void GravityMultiply()
    {   //Makes
        if(!isGrounded || rb.velocity.y < 0f)
        {
            rb.gravityScale = gravityScale * gravityMultiplier;
        }
        else
        {
            rb.gravityScale = gravityScale;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (string collider in collisionIgnores)
        {
            if (collision.gameObject.CompareTag(collider))
            {
                Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(groundPoint.position, circleRadius);
        Gizmos.DrawSphere(wallPoint.position, circleRadius);
        Gizmos.DrawWireCube(new Vector2(transform.position.x , transform.position.y - castDistance), boxSize);
        Gizmos.DrawWireCube(transform.position, seeSize);
    }
}
