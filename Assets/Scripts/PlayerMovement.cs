using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public enum MovementState
{
    Normal,
    Dashing,
    Climbing,
    WallSlide
}
public class PlayerMovement : MonoBehaviour
{
    MovementState playerState;

    [SerializeField] float moveSpeed = 5f;
    
    //Jump variables
    [SerializeField] float jumpForce = 11f;
    [SerializeField] float jumpCut;

    [SerializeField] float coyoteTime = 0.15f;
    private float coyoteTimeCounter;

    [SerializeField] float jumpBufferTime;
    private float jumpBufferCounter;

    [SerializeField] float gravityMultiplier;
    [SerializeField] float gravityScale;
    [SerializeField] int gravitySlideCheck = 0;

    //Dash variables
    [SerializeField] TrailRenderer dashTrail;
    private bool canDash = true;
    private bool dashRequest = false;
    [SerializeField] float dashPower;
    [SerializeField] float dashingTime;
    [SerializeField] float dashingCoolDown;


    [SerializeField] float climbingSpeed;

    [SerializeField] Animator anim;

    private Rigidbody2D rb;
    [SerializeField] Vector2 boxSize;
    [SerializeField] float boxIntercept;
    [SerializeField] float castDistance;
    [SerializeField] LayerMask groundLayer;

    [SerializeField] Transform wallCheck;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] float wallSlidingSpeed;

    private float horizontalInput;
    private float verticalInput;

    private int maxhealth = 1;
    [SerializeField] int currenthealth;


    void Start()
    {

        currenthealth = maxhealth;
        rb = GetComponent<Rigidbody2D>();

    }

    private void PlayerInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }
  
    void Update()
    {
        PlayerInput();
        //Prevents Player from moving during dash
        
        CheckJump();
        CheckDash();
        CheckWallSlide();
        CheckClimbing();
    }

    private void FixedUpdate()
    {
        Physics2D.IgnoreLayerCollision(8, 6, true);
        //Prevents Player from moving during dash
        switch (playerState)
        {
            case MovementState.Normal:
                GravityMultiply();
                ApplyJump();
                Run();
                break;
            case MovementState.Dashing:
                ApplyDash();
                break;
            case MovementState.Climbing:
                ApplyClimbing();
                Run();
                break;
            case MovementState.WallSlide:
                ApplyWallSlide();
                Run();
                break;
            default:
                GravityMultiply();
                ApplyJump();
                Run();
                break;
        }
    }

    private void Run()
    {
        rb.velocity = new Vector2(moveSpeed * horizontalInput, rb.velocity.y);
    }
    #region
    private void CheckClimbing()
    {
        if (Mathf.Abs(verticalInput) > 0f)
        {
            Debug.Log("touchedboth");
            playerState = MovementState.Climbing;
        }
    }
    private void ApplyClimbing()
    {
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(rb.velocity.x, verticalInput * climbingSpeed);
        Debug.Log("climbing");
    }
    #endregion

    #region Dash
    private void CheckDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            dashRequest = true;
            playerState = MovementState.Dashing;
        }
    }
    private void ApplyDash()
    {
        if (dashRequest)
        {
            dashRequest = false;
            StartCoroutine(Dash());
        }
    }
    private IEnumerator Dash()
    {
        canDash = false;

        rb.gravityScale = 0;
        rb.velocity = new Vector2(horizontalInput * dashPower, 0f);
        dashTrail.emitting = true;

        yield return new WaitForSeconds(dashingTime);
        dashTrail.emitting = false;

        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.gravityScale = gravityScale;
        playerState = MovementState.Normal;

        yield return new WaitForSeconds(dashingCoolDown);
        canDash = true;
    }
    #endregion

    #region Wall Slide
    private bool isWalled()
    { 
        return Physics2D.OverlapCircle(transform.position, 0.5f, wallLayer);
    }
    private void CheckWallSlide()
    {
        if (isWalled() &&horizontalInput != 0 && !IsGrounded())
        {
            playerState = MovementState.WallSlide;
            Debug.Log("its waling");
        }
        else
        {
            if (playerState == MovementState.WallSlide)
            {
                playerState = MovementState.Normal;
            }
        }
    }
    private void ApplyWallSlide()
    {
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
    }
    #endregion

    #region Jump
    public bool IsGrounded()
    {
        return Physics2D.BoxCast(new Vector2(transform.position.x - boxIntercept, transform.position.y), boxSize, 0, -transform.up, castDistance, groundLayer);
    }
    private void CheckJump()
    {
        if (playerState == MovementState.Dashing)
        {
            return;
        }
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

        //Creates extra time before landing to make jumping smoother when pressing jump quickly
        if (Input.GetButton("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }
    private void ApplyJump()
    {
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
    }
    private void GravityMultiply()
    {
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = gravityScale * gravityMultiplier;
        }
        else
        {
            rb.gravityScale = gravityScale;
        }
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(new Vector2(transform.position.x - boxIntercept, transform.position.y - castDistance), boxSize);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            playerState = MovementState.Climbing;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            playerState = MovementState.Normal;
        }
    }
}