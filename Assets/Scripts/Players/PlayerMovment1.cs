using System;
using UnityEngine;

public class PlayerMovment1 : MonoBehaviour
{
    [Header("References")]
    public PlayerMovementStats MoveStats;
    [SerializeField] private Collider2D _feetColl;
    [SerializeField] private Collider2D _bodyColl;
    [SerializeField] private Collider2D _headColl;
    [SerializeField] private GameObject waterline;

    private Rigidbody2D _rb;

    // Movement variables
    public float horizontalVelocity { get; private set; }
    public float VerticalVelocity { get; private set; }
    private bool _isFacingRight;

    // Swimming variables
    private bool _isSwimming;

    // Dash variables
    [Header("Dash Settings")]
    [Tooltip("Time (seconds) before you can dash again.")]
    public float dashCooldown = 1f;

    private bool _isDashing;
    private float _dashTimer;
    private Vector2 _dashDirection;

    // Tracks when you can dash again
    private float _dashCooldownTimer;

    // Input manager
    private InputManager inputManager;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        _isFacingRight = true;
        _rb = GetComponent<Rigidbody2D>();
        _isSwimming = true;
    }

    private void Update()
    {
        // Decrement dash cooldown timer
        if (_dashCooldownTimer > 0f)
        {
            _dashCooldownTimer -= Time.deltaTime;
        }

        // Check water conditions
        CheckHeadAboveWater();

        // Only allow dash input if we're swimming
        if (_isSwimming)
        {
            DashCheck();
        }
    }

    private void FixedUpdate()
    {
        // Handle swimming movement
        SwimMovement();

        // If we are dashing, override normal movement
        if (_isDashing)
        {
            Dash();
        }
        else
        {
            // Apply normal swimming velocity
            _rb.velocity = new Vector2(horizontalVelocity, VerticalVelocity);
        }
    }

    #region Movement

    private void TurnCheck(Vector2 moveInput)
    {
        if (_isFacingRight && moveInput.x < 0)
        {
            Turn(false);
        }
        else if (!_isFacingRight && moveInput.x > 0)
        {
            Turn(true);
        }
    }

    private void Turn(bool turnRight)
    {
        _isFacingRight = turnRight;
        transform.localScale = new Vector3(turnRight ? 1 : -1, 1, 1); // Flip the character horizontally
    }

    #endregion

    #region Swimming

    private void SwimMovement()
    {
        Vector2 moveInput = inputManager.Movement;

        // Horizontal Swimming
        float targetSwimSpeed = moveInput.x * MoveStats.swimMaxSpeed;
        horizontalVelocity = Mathf.Lerp(
            horizontalVelocity,
            targetSwimSpeed,
            MoveStats.swimAcceleration * Time.fixedDeltaTime
        );

        // Vertical Swimming
        float targetVerticalSwimSpeed;
        if (moveInput.y > 0) // Moving up
        {
            targetVerticalSwimSpeed = moveInput.y * MoveStats.swimUpwardSpeed;
        }
        else if (moveInput.y < 0) // Moving down
        {
            targetVerticalSwimSpeed = moveInput.y * MoveStats.swimDownwardSpeed;
        }
        else
        {
            // No input -> apply gravity-like sinking
            if (VerticalVelocity > 0)
            {
                VerticalVelocity = 0;
            }
            targetVerticalSwimSpeed = VerticalVelocity - (MoveStats.gravityFeeling * Time.fixedDeltaTime);
            targetVerticalSwimSpeed = Mathf.Clamp(targetVerticalSwimSpeed, -MoveStats.maxSinkSpeed, 0);
        }

        // Smooth interpolation for vertical velocity
        VerticalVelocity = Mathf.Lerp(
            VerticalVelocity,
            targetVerticalSwimSpeed,
            MoveStats.swimAcceleration * Time.fixedDeltaTime
        );

        // Flip character based on input
        TurnCheck(moveInput);
    }

    public void OnEnterWater(float verticalSpeed, float speedMultiplier)
    {
        _isSwimming = true;
        _rb.gravityScale = MoveStats.waterGravityScale; // Apply water gravity
        Debug.Log("Player entered swimming mode.");
    }

    public void OnExitWater()
    {
        _isSwimming = false;
        _rb.gravityScale = MoveStats.defaultGravityScale; // Restore normal gravity
        horizontalVelocity = 0f;
        VerticalVelocity = 0f; // Stop all movement
        Debug.Log("Player exited swimming mode.");
    }

    private void CheckHeadAboveWater()
    {
        if (_headColl != null && waterline != null)
        {
            // Y position of the player's head collider
            float headPositionY = _headColl.bounds.max.y;
            // Y position of the waterline GameObject
            float waterLineY = waterline.transform.position.y;

            // Compare head position with the waterline
            if (headPositionY > waterLineY && _isSwimming)
            {
                OnExitWater(); 
            }
            else if (headPositionY <= waterLineY && !_isSwimming)
            {
                OnEnterWater(MoveStats.swimVerticalSpeed, MoveStats.swimAcceleration);
            }
        }
    }

    #endregion

    #region Dash

    private void DashCheck()
    {
        // Ensure dash input is pressed, not currently dashing, and cooldown has elapsed
        if (inputManager.DashWasPressed && !_isDashing && _dashCooldownTimer <= 0f)
        {
            StartDash();
        }
    }

    private void StartDash()
    {
        _isDashing = true;
        _dashTimer = 0f;

        // Reset the cooldown so we can't dash again immediately
        _dashCooldownTimer = dashCooldown;

        // Use movement input as the dash direction
        _dashDirection = inputManager.Movement.normalized;

        // If no directional input, default to facing direction
        if (_dashDirection == Vector2.zero)
        {
            _dashDirection = _isFacingRight ? Vector2.right : Vector2.left;
        }

        Debug.Log("Dash started! Direction: " + _dashDirection);
    }

    private void Dash()
    {
        _dashTimer += Time.fixedDeltaTime;

        // During the dash, set velocity to dashDirection * MoveStats.DashSpeed
        _rb.velocity = _dashDirection * MoveStats.DashSpeed;

        // End dash after the dash duration from MoveStats
        if (_dashTimer >= MoveStats.DashTime)
        {
            _isDashing = false;
            horizontalVelocity = 0f;
            VerticalVelocity = 0f;
            Debug.Log("Dash ended.");
        }
    }

    #endregion
}
