using UnityEngine;
using UnityEngine.InputSystem;

public class DinoController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    [Header("Game Manager")]
    public GameManager gameManager;

    [Header("Cài đặt nhảy")]
    [InspectorName("Lực nhảy")]
    public float jumpForce = 11f;

    [InspectorName("Thời gian giữ để nhảy cao hơn")]
    public float maxHoldJumpTime = 0.25f;

    [InspectorName("Trọng lực khi đang giữ nhảy")]
    public float holdGravityMultiplier = 0.45f;

    [InspectorName("Hệ số rơi nhanh")]
    public float fallMultiplier = 3f;

    [InspectorName("Hệ số rơi khi thả sớm")]
    public float releaseJumpMultiplier = 3.5f;

    [InspectorName("Tốc độ rơi tối đa")]
    public float maxFallSpeed = 18f;

    [Header("Kiểm tra chạm đất")]
    public Transform groundCheck;

    [InspectorName("Bán kính kiểm tra đất")]
    public float groundCheckRadius = 0.2f;

    [InspectorName("Layer mặt đất")]
    public LayerMask whatIsGround;

    private bool isGrounded;
    private bool isDead;

    private bool isJumping;
    private bool isHoldingJump;
    private float holdJumpCounter;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }

        CheckGround();
        HandleJumpInput();
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            return;
        }

        ApplyBetterJumpGravity();
    }

    private void CheckGround()
    {
        if (groundCheck == null)
        {
            Debug.LogWarning("Chưa gán GroundCheck trong DinoController.");
            return;
        }

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            whatIsGround
        );

        if (isGrounded && rb.linearVelocity.y <= 0f)
        {
            isJumping = false;
            isHoldingJump = false;
            holdJumpCounter = 0f;
        }
    }

    private void HandleJumpInput()
    {
        if (IsJumpPressedDown() && isGrounded)
        {
            Jump();
        }

        if (IsJumpHolding() && isJumping && holdJumpCounter > 0f)
        {
            isHoldingJump = true;
        }

        if (IsJumpReleased())
        {
            isHoldingJump = false;
            holdJumpCounter = 0f;
        }
    }

    private void Jump()
    {
        isJumping = true;
        isHoldingJump = true;
        holdJumpCounter = maxHoldJumpTime;

        Vector2 velocity = rb.linearVelocity;
        velocity.y = jumpForce;
        rb.linearVelocity = velocity;

        if (anim != null)
        {
            anim.SetBool("isJumping", true);
        }
    }

    private void ApplyBetterJumpGravity()
    {
        Vector2 velocity = rb.linearVelocity;

        if (velocity.y > 0f)
        {
            if (isHoldingJump && holdJumpCounter > 0f)
            {
                // Giữ chạm: giảm trọng lực để nhảy cao hơn tự nhiên
                rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (holdGravityMultiplier - 1f) * Time.fixedDeltaTime;
                holdJumpCounter -= Time.fixedDeltaTime;
            }
            else
            {
                // Thả sớm: tăng trọng lực để rơi xuống nhanh hơn
                rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (releaseJumpMultiplier - 1f) * Time.fixedDeltaTime;
            }
        }
        else if (velocity.y < 0f)
        {
            // Khi rơi: rơi nhanh hơn, không bị cảm giác bay lơ lửng
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        }

        if (rb.linearVelocity.y < -maxFallSpeed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -maxFallSpeed);
        }
    }

    private void UpdateAnimation()
    {
        if (anim == null)
        {
            return;
        }

        if (isGrounded && rb.linearVelocity.y <= 0f)
        {
            anim.SetBool("isJumping", false);
        }
    }

    private bool IsJumpPressedDown()
    {
        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            return true;
        }

        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            return true;
        }

        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            return true;
        }

        return false;
    }

    private bool IsJumpHolding()
    {
        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.isPressed)
        {
            return true;
        }

        if (Mouse.current != null &&
            Mouse.current.leftButton.isPressed)
        {
            return true;
        }

        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.isPressed)
        {
            return true;
        }

        return false;
    }

    private bool IsJumpReleased()
    {
        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.wasReleasedThisFrame)
        {
            return true;
        }

        if (Mouse.current != null &&
            Mouse.current.leftButton.wasReleasedThisFrame)
        {
            return true;
        }

        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasReleasedThisFrame)
        {
            return true;
        }

        return false;
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        isJumping = false;
        isHoldingJump = false;

        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        if (anim != null)
        {
            anim.SetBool("isJumping", false);
            anim.SetBool("isDead", true);
        }

        if (gameManager != null)
        {
            gameManager.GameOver();
        }
        else
        {
            Debug.LogWarning("Chưa gán GameManager vào DinoController.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            Die();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}