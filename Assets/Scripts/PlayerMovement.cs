using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    public float moveSpeed = 5f;

    //Jump variables
    public float jumpForce = 11f;
    public float jumpCut;
    public float coyoteTime = 0.15f;
    private float coyoteTimeCounter;
    public float jumpBufferTime;
    private float jumpBufferCounter;
    public float gravityMultiplier;
    public float gravityScale;
    public int gravitySlideCheck = 0;

    //Dash variables
    public TrailRenderer dashTrail;
    private bool canDash = true;
    private bool isDashing = false;
    public float dashPower;
    public float dashingTime;
    public float dashingCoolDown;

    private bool isClimbing = false;
    private bool isLadder = false;
    public float climbingSpeed;


    public Animator anim;

    private Rigidbody2D rb;
    public Vector2 boxSize;
    public float boxIntercept;
    public float castDistance;
    public LayerMask groundLayer;

    public Transform wallCheck;
    public LayerMask wallLayer;
    private bool isWallSliding = false;
    public float wallSlidingSpeed;

    private float horizontalInput;
    private float verticalInput;

    private int maxhealth = 1;
    public int currenthealth;


    private void Awake()
    {
        instance = this;
    }
    void Start()
    {

        currenthealth = maxhealth;
        rb = GetComponent<Rigidbody2D>();

    }

    public bool IsGrounded()
    {
        return Physics2D.BoxCast(new Vector2(transform.position.x - boxIntercept, transform.position.y), boxSize, 0, -transform.up, castDistance, groundLayer);
    }


    private bool isWalled()
    {
        return Physics2D.OverlapCircle(transform.position, 0.5f, wallLayer);
    }

    private void wallSlide()
    {
        if(isWalled() && !IsGrounded() && horizontalInput != 0)
        {
            isWallSliding = true;
            Debug.Log("its waling");
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
            return;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(new Vector2(transform.position.x - boxIntercept, transform.position.y - castDistance), boxSize);
    }

    void Update()
    {
        // Handle player input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        wallSlide();
        CheckDashing();
        CheckJump();
        CheckClimbing();
    }

    private void FixedUpdate()
    {
        Physics2D.IgnoreLayerCollision(8, 6, true);


        if (isClimbing)
        {
            rb.gravityScale = 0f;
            rb.velocity = new Vector2(rb.velocity.x, verticalInput * climbingSpeed);
            Debug.Log("climbing");
        }
        else
        {
            rb.gravityScale = gravityScale;
        }

        Run(moveSpeed);
        // Handle jumping
        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpBufferCounter = 0f;
        }
        if (Input.GetButtonUp("Jump"))
        {
            if (Mathf.Abs(rb.velocity.y) > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCut);
            }
            coyoteTimeCounter = 0f;
        }

        if (Mathf.Abs(rb.velocity.y) < 0)
        {
            rb.gravityScale = gravityScale * gravityMultiplier;
        }
    }

    private void CheckClimbing()
    {
        if(isLadder && Mathf.Abs(verticalInput) > 0f)
        {
            Debug.Log("touchedboth");
            isClimbing = true;
        }
    }
    
    private void CheckJump()
    {
        //Creates extra time before landing to make jumping smoother when pressing jump quickly
        if (Input.GetButton("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
        //Creates extra JumpTime jumping off the ledge smoother by creating extra time to jump off-land
        if (IsGrounded() == true)
        {
            coyoteTimeCounter = coyoteTime;
            rb.gravityScale = gravitySlideCheck;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
            rb.gravityScale = gravityScale;
        }
    }
    private void CheckDashing()
    {
         if (isDashing)
         {
            return;
         }
        
         if (Input.GetKeyDown(KeyCode.LeftShift))
         {
            if(canDash)
            {
                StartCoroutine(Dash());
            }
         }
    }

    private void Run(float movementSpeed)
    {
        rb.velocity = new Vector2(moveSpeed * horizontalInput, rb.velocity.y);
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        rb.velocity = new Vector2(horizontalInput * dashPower, 0f);
        dashTrail.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        dashTrail.emitting = false;
        isDashing = false;
        yield return new WaitForSeconds(dashingCoolDown);
        canDash = true;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isLadder = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isLadder = false;
            isClimbing = false;
        }
    }
}
