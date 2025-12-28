using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;

    [Header("Movement")]
    public float moveSpeed = 5f;
    private float horizontalInput;
    private float verticalInput;

    [Header("Jump")]
    public float jumpForce = 11f;
    public float jumpCut = 0.5f;
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.15f;
    private float coyoteCounter;
    private float jumpBufferCounter;

    [Header("Gravity")]
    public float gravityScale = 3f;
    public float gravityMultiplier = 2f;

    [Header("Dash")]
    public float dashPower = 12f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;
    public TrailRenderer dashTrail;
    private bool canDash = true;
    private bool isDashing;

    [Header("Climbing")]
    public float climbSpeed = 4f;
    private bool isClimbing;
    private bool isLadder;

    [Header("Wall Slide")]
    public LayerMask wallLayer;
    public float wallSlideSpeed = 2f;
    private bool isWallSliding;

    [Header("Ground Check")]
    public Vector2 boxSize;
    public float castDistance;
    public LayerMask groundLayer;

    private Rigidbody2D rb;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        ReadInput();
        HandleJumpInput();
        HandleDashInput();
        HandleWallSlide();
        HandleClimbingCheck();
    }

    private void FixedUpdate()
    {
        Move();
        ApplyJump();
        ApplyClimbing();
        ApplyGravity();
    }

    #region Input

    private void ReadInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void HandleJumpInput()
    {
        jumpBufferCounter = Input.GetButton("Jump")
            ? jumpBufferTime
            : jumpBufferCounter - Time.deltaTime;

        coyoteCounter = IsGrounded()
            ? coyoteTime
            : coyoteCounter - Time.deltaTime;
    }

    private void HandleDashInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    #endregion

    #region Movement

    private void Move()
    {
        if (isDashing) return;
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
    }

    private void ApplyJump()
    {
        if (jumpBufferCounter > 0 && coyoteCounter > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpBufferCounter = 0;
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCut);
        }
    }

    private void ApplyGravity()
    {
        if (rb.velocity.y < 0)
            rb.gravityScale = gravityScale * gravityMultiplier;
        else
            rb.gravityScale = gravityScale;
    }

    #endregion

    #region Climbing

    private void HandleClimbingCheck()
    {
        isClimbing = isLadder && Mathf.Abs(verticalInput) > 0;
    }

    private void ApplyClimbing()
    {
        if (!isClimbing) return;

        rb.gravityScale = 0f;
        rb.velocity = new Vector2(rb.velocity.x, verticalInput * climbSpeed);
    }

    #endregion

    #region Wall Slide

    private void HandleWallSlide()
    {
        if (IsTouchingWall() && !IsGrounded() && horizontalInput != 0)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(
                rb.velocity.x,
                Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue)
            );
        }
        else
        {
            isWallSliding = false;
        }
    }

    private bool IsTouchingWall()
    {
        return Physics2D.OverlapCircle(transform.position, 0.5f, wallLayer);
    }

    #endregion

    #region Dash

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        rb.velocity = new Vector2(horizontalInput * dashPower, 0);
        dashTrail.emitting = true;

        yield return new WaitForSeconds(dashDuration);

        dashTrail.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    #endregion

    #region Ground Check

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(
            transform.position,
            boxSize,
            0,
            Vector2.down,
            castDistance,
            groundLayer
        );
    }

    #endregion

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Ladder"))
            isLadder = true;
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Ladder"))
        {
            isLadder = false;
            isClimbing = false;
        }
    }
}
