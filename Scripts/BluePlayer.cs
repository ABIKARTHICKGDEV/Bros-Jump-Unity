

using System;
using Unity.Mathematics;
using UnityEngine;

public class BluePlayer : PlayerBase {
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
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private int maxJumps = 2;

    [Header("Effects")]
    [SerializeField] private GameObject wallHitEffectPrefab;

    [Header("Spin")]
    [SerializeField] private float spinSpeed = 1080f;
    private bool startSpin;
    private float currentSpinAmount;
    private int spinDirection;

    [Header("Jump Assist")]
    [SerializeField] private float jumpGroundIgnoreTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.15f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
  
    private bool movementLocked;

    //public event Action OnTouched;
    private int direction = 1;

    private int jumpCount;
    private bool isGrounded;
    //public bool IsGrounded_partical => isGrounded;

    private float jumpTimer;
    private float jumpBufferCounter;

    

    private bool wasTouchingWall;

    private void Update() {
        HandleGroundCheck();
       
        HandleJumpInput();
        
        HandleBufferedJump();
        HandleSpin();
        
    }

    private void FixedUpdate() {
      
        HandleWallCheck();
        Move();
    }

    private void HandleGroundCheck() {
        bool currentlyGrounded =
            jumpTimer <= 0f && IsGrounded();

        if (currentlyGrounded && !isGrounded) {
            jumpCount = 0;
            startSpin = false;

            visual.localRotation =
                Quaternion.identity;

           
        }

        isGrounded = currentlyGrounded;

        jumpTimer -= Time.deltaTime;
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
            //AudioManager.Instance.PlayWallHit();
        }
        if (hitWall && !wasTouchingWall && !isGrounded) { SpawnWallEffect(); } // hit wall effect

        wasTouchingWall = hitWall;
        
    }

    private void HandleJumpInput() {
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
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
        if (jumpBufferCounter > 0f &&
            jumpCount < maxJumps) {
            Jump();
            jumpBufferCounter = 0f;
        }
    }

    private void Jump() {

        OnArrowLock();

        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            0f);

        rb.AddForce(
            Vector2.up * jumpForce,
            ForceMode2D.Impulse);

        jumpCount++;

        if (jumpCount == 2) {
            startSpin = true;
            currentSpinAmount = 0f;

            // Store direction at jump time
            spinDirection = direction;
        }

        jumpTimer = jumpGroundIgnoreTime;
        AudioManager.Instance.PlayJump();
    }

    private void HandleSpin() {
        if (!startSpin)
            return;

        float rotationThisFrame =
            visual.localScale.x * -spinSpeed * Time.deltaTime;

        visual.Rotate(
            0f,
            0f,
            rotationThisFrame);

        currentSpinAmount += Mathf.Abs(rotationThisFrame);

        if (currentSpinAmount >= 360f) {
            startSpin = false;

            visual.localRotation =
                Quaternion.identity;
        }
    }

    private void Move() {
        if (movementLocked)
            return;
        Vector2 velocity = rb.linearVelocity;
        velocity.x = direction * moveSpeed;
        rb.linearVelocity = velocity;
    }

    private void FlipDirection() {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapBox(
            groundCheck.position,
            groundCheckSize,
            0f,
            groundLayer);
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
        if (direction > 0) {
            Vector3 scale = effect.transform.localScale;
            scale.x *= -1;
            effect.transform.localScale = scale;
        }
        if (visual.localScale.x > 0) {
            Vector3 scale = effect.transform.localScale;
            scale.x *= -1;
            effect.transform.localScale = scale;
        }
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

    private void OnDrawGizmosSelected() {
        if (groundCheck != null) {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(
     groundCheck.position,
     groundCheckSize);
        }

        if (frontWallCheck != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(
                frontWallCheck.position,
                new Vector2(wallCheckRadiusX, wallCheckRadiusY));
        }
    }
}