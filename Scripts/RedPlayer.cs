using UnityEngine;

public class RedPlayer : PlayerBase {
    [Header("References")]
    [SerializeField] private Transform visual;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform frontWallCheck;

   

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("Checks")]
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.4f, 0.1f);
    [SerializeField] private float wallCheckRadiusX = 0.1f;
    [SerializeField] private float wallCheckRadiusY = 0.1f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 10f;

    [Header("Effects")]
    [SerializeField] private GameObject wallHitEffectPrefab;

    [Header("Input")]
    [SerializeField] private float jumpBufferTime = 0.15f;
    private bool movementLocked;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
   

    private int direction = -1;

    private bool canJump = true;
    private bool isGrounded;

    private float jumpBufferCounter;

    private bool wasTouchingWall;

    private void Update() {
        HandleGroundCheck();
       
        HandleJumpInput();
        HandleBufferedJump();
    }

    private void FixedUpdate() {
        HandleWallCheck();
        Move();
        
    }

    private void HandleGroundCheck() {
        bool currentlyGrounded = IsGrounded();

        // Landed
        if (currentlyGrounded && !isGrounded) {
            canJump = true;
        }

        // Left ground without jumping
        if (!currentlyGrounded && isGrounded) {
            canJump = false;
        }

        isGrounded = currentlyGrounded;
    }

    private void HandleWallCheck() {
        bool hitWall = Physics2D.OverlapBox(
            frontWallCheck.position,
            new Vector2(wallCheckRadiusX, wallCheckRadiusY),
            0f,
            wallLayer);

        if (hitWall && !wasTouchingWall) {
           
            RaiseTouched();

            direction *= -1;

            FlipDirection();

            canJump = true;
          //  AudioManager.Instance.PlayWallHit();
        }
        if (hitWall && !wasTouchingWall && !isGrounded) { SpawnWallEffect(); } // hit wall effect

        wasTouchingWall = hitWall;
    }

    private void SpawnWallEffect() {
        if (wallHitEffectPrefab == null)
            return;

        GameObject effect = Instantiate(
            wallHitEffectPrefab,
            frontWallCheck.position,
            Quaternion.identity
        );

        // Flip effect when facing left
        //if (direction > 0) {
        //    Vector3 scale = effect.transform.localScale;
        //    scale.x *= -1;
        //    effect.transform.localScale = scale;
        //}
        if (visual.localScale.x > 0) {
            Vector3 scale = effect.transform.localScale;
            scale.x *= -1;
            effect.transform.localScale = scale;
        }
    }

    private void HandleJumpInput() {
        if (Input.GetKeyDown(KeyCode.W)) {
            jumpBufferCounter = jumpBufferTime;
        }

        jumpBufferCounter -= Time.deltaTime;
    }
    public void MobileJumpPressed()
    {
        jumpBufferCounter = jumpBufferTime;

        jumpBufferCounter -= Time.deltaTime;
    }

    private void HandleBufferedJump() {
        if (canJump && jumpBufferCounter > 0f) {

            Jump();

            jumpBufferCounter = 0f;
        }
    }

    private void Jump() {

        OnArrowLock();

        canJump = false;

        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            0f
        );

        Vector2 jumpDirection =
            new Vector2(direction, 1f).normalized;

        rb.AddForce(
            jumpDirection * jumpForce,
            ForceMode2D.Impulse
        );
        AudioManager.Instance.PlayJump();
    }

    private void Move() {

        if (movementLocked)
            return;

        Vector2 velocity = rb.linearVelocity;

        velocity.x = direction * moveSpeed;

        rb.linearVelocity = velocity;
    }

    private void FlipDirection() {
        Vector3 scale = visual.localScale;
        scale.x *= -1;
        visual.localScale = scale;
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapBox(
            groundCheck.position,
            groundCheckSize,
            0f,
            groundLayer);
    }
    public override void SetMovementLocked(bool value) 
    { 
        movementLocked = value;

        if (value) {
            rb.linearVelocity = Vector2.zero;

            rb.constraints =
                RigidbodyConstraints2D.FreezePositionX |
                RigidbodyConstraints2D.FreezeRotation;
        } else {
            rb.constraints =
                RigidbodyConstraints2D.FreezeRotation;
        }
    }

    private void OnDrawGizmos() {
        if (groundCheck != null) {
            Gizmos.color = Color.red;

            Gizmos.DrawWireCube(
      groundCheck.position,
      groundCheckSize);
        }

        if (frontWallCheck != null) {
            Gizmos.color = Color.yellow;

            Gizmos.DrawWireCube(
                frontWallCheck.position,
                new Vector2(wallCheckRadiusX, wallCheckRadiusY)
            );
        }
    }
}