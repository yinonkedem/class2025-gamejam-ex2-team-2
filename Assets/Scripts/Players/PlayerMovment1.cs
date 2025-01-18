using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovment1 : MonoBehaviour
{
    [Header("References")]
    public PlayerMovementStats MoveStats;
    [SerializeField] private Collider2D _feetColl;
    [SerializeField] private Collider2D _bodyColl;

    private Rigidbody2D _rb;

    // Movement variables
    public float horizontalVelocity {get; private set;}
    private bool _isFacingRight;

    // Collision check variables
    private RaycastHit2D _groundHit;
    private RaycastHit2D _headHit;
    private RaycastHit2D _wallHit;
    private RaycastHit2D _lastWallHit;
    private bool _isGrounded;
    private bool _bumpedHead;
    private bool _isTouchingWall;
    
    // Jump vars
    public float VerticalVelocity { get; private set; }
    private bool _isJumping;
    private bool _isFastFalling;
    private bool _isFalling;
    private float _fastFallTime;
    private float _fastFallReleaseSpeed;
    private int _numberOfJumpsUsed;

    // Apex vars
    private float _apexPoint;
    private float _timePastApexThreshold;
    private bool _isPastApexThreshold;

    // Jump buffer vars
    private float _jumpBufferTimer;
    private bool _jumpReleasedDuringBuffer;

    // Coyote time vars
    private float _coyoteTimer;
    
    // wall slide vars
    private bool _isWallSliding;
    private bool _isWallSlideFalling;
    
    // Wall jump vars
    private bool _useWallJumpMoveStats;
    private bool _isWallJumping;
    private float _wallJumpTime;
    private bool _isWallJumpFastFalling;
    private bool _isWallJumpFalling;
    private float _wallJumpFastFallTime;
    private float _wallJumpFastFallReleaseSpeed;

    private float _wallJumpPostBufferTimer;
    private float _wallJumpApexPoint;
    private float _timePastWallJumpApexThreshold;
    private bool _isPastWallJumpApexThreshold;

    // Dash vars
    private bool _isDashing;
    private bool _isAirDashing;
    private float _dashTimer;
    private float _dashOnGroundTimer;
    private int _numberOfDashesUsed;
    private Vector2 _dashDirection;
    private bool _isDashFastFalling;
    private float _dashFastFallTime;
    private float _dashFastFallReleaseSpeed;
    
    // Input manager
    private InputManager inputManager;
    
    // Swimming vars
    private float _swimmingSpeedMultiplier = 1f;
    private float _swimmingVerticalSpeed = 0f;
    private bool _isSwimming = false;
    
    private void Update()
    {
        CountTimers();
        JumpCheck();
        LandCheck();
        WallJumpCheck();
        
        WallSlideCheck();
        DashCheck();
    }

    private void FixedUpdate()
    {
        CollisionChecks();
        Jump();
        Fall();
        WallSlide();
        WallJump();
        Dash();
        
        if (_isSwimming)
        {
            SwimMovement();
        }
        else
        {
            if (_isGrounded)
            {
                Move(MoveStats.groundAcceleration, MoveStats.groundDeceleration, inputManager.Movement);
            }
            else
            {
                // Wall jumping
                if (_useWallJumpMoveStats)
                {
                    Move(MoveStats.wallJumpMoveAcceleration, MoveStats.wallJumpMoveDeceleration, inputManager.Movement);
                }
                // Airborne
                else
                {
                    Move(MoveStats.airAcceleration, MoveStats.airDeceleration, inputManager.Movement);
                }
            }
        }

        ApplyVelocity();
    }
    
    private void ApplyVelocity()
    {
        // CLAMP FALL SPEED
        if (!_isDashing)
        {
            VerticalVelocity = Mathf.Clamp(VerticalVelocity,
                -MoveStats.MaxFallSpeed, 50f);
        }
        else
        {
            VerticalVelocity = Mathf.Clamp(VerticalVelocity, -50f, 50f);
        }

        _rb.velocity = new Vector2(horizontalVelocity, VerticalVelocity);
    }
    
    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        _isFacingRight = true;
        _rb = GetComponent<Rigidbody2D>();
    }

    #region Movement

    private void Move(float acceleration, float deceleration, Vector2 moveInput)
    {
        if (!_isDashing)
        {
            if (Mathf.Abs(moveInput.x) >= MoveStats.moveThreshold)
            {
                TurnCheck(moveInput);

                float targetVelocity = 0f;
                if (inputManager.RunIsHeld)
                {
                    targetVelocity = moveInput.x * MoveStats.maxRunSpeed;
                }
                else
                {
                    targetVelocity = moveInput.x * MoveStats.maxWalkSpeed;
                }

                horizontalVelocity = Mathf.Lerp(horizontalVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            }
            else if (Mathf.Abs(moveInput.x) < MoveStats.moveThreshold)
            {
                horizontalVelocity = Mathf.Lerp(horizontalVelocity, 0f, deceleration * Time.fixedDeltaTime);
            }
        }
    }
    
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
        if (turnRight)
        {
            _isFacingRight = true;
            transform.Rotate(0f, 180f, 0f);
        }
        else
        {
            _isFacingRight = false;
            transform.Rotate(0f, -180f, 0f);
        }
    }
    
    #endregion
    
    #region Swimming

    private void SwimMovement()
    {
        Vector2 moveInput = inputManager.Movement;

        // Horizontal Swimming (based on Move logic)
        if (Mathf.Abs(moveInput.x) >= MoveStats.moveThreshold)
        {
            TurnCheck(moveInput);

            float targetSwimSpeed = moveInput.x * MoveStats.swimMaxSpeed;
            horizontalVelocity = Mathf.Lerp(horizontalVelocity, targetSwimSpeed, MoveStats.swimAcceleration * Time.fixedDeltaTime);
        }
        else
        {
            horizontalVelocity = Mathf.Lerp(horizontalVelocity, 0f, MoveStats.swimDeceleration * Time.fixedDeltaTime);
        }

        // Vertical Swimming (based on Move logic)
        if (Mathf.Abs(moveInput.y) >= MoveStats.moveThreshold)
        {
            float targetVerticalSwimSpeed = moveInput.y * MoveStats.swimVerticalSpeed;
            VerticalVelocity = Mathf.Lerp(VerticalVelocity, targetVerticalSwimSpeed, MoveStats.swimAcceleration * Time.fixedDeltaTime);
        }
        else
        {
            VerticalVelocity = Mathf.Lerp(VerticalVelocity, 0f, MoveStats.swimDeceleration * Time.fixedDeltaTime);
        }

        // Adjust gravity for swimming
        _rb.gravityScale = MoveStats.waterGravityScale;
    }



    public void OnEnterWater(float verticalSpeed, float speedMultiplier)
    {
        _isSwimming = true;
        _swimmingVerticalSpeed = verticalSpeed;
        _swimmingSpeedMultiplier = speedMultiplier;
        _rb.gravityScale = MoveStats.waterGravityScale;
        Debug.Log("Player entered swimming mode.");
    }

    public void OnExitWater()
    {
        _isSwimming = false;
        _rb.gravityScale = MoveStats.defaultGravityScale; // Restore normal gravity
        _isJumping = false;
        _isFastFalling = false;
        Debug.Log("Exited swimming mode. Regular jump restored.");
    }

    private IEnumerator SmoothGravityChange(float targetGravity)
    {
        float duration = 0.5f; // Duration of the transition
        float elapsed = 0f;
        float initialGravity = _rb.gravityScale;

        while (elapsed < duration)
        {
            _rb.gravityScale = Mathf.Lerp(initialGravity, targetGravity, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _rb.gravityScale = targetGravity;
    }

    #endregion
    
    #region Land/Fall

    private void LandCheck()
    {
        // LANDED
        if ((_isJumping || _isFalling || _isWallJumpFalling || _isWallJumping || _isWallSlideFalling || _isWallSliding || _isDashFastFalling) && _isGrounded && VerticalVelocity <= 0f)
        {
            ResetJumpValues();
            StopWallSlide();
            ResetWallJumpValues();
            ResetDashValues();
            
            _numberOfJumpsUsed = 0;

            VerticalVelocity = Physics2D.gravity.y;
            if (_isDashFastFalling && _isGrounded)
            {
                ResetDashValues();
                return;
            }
            ResetDashValues();
        }
    }

    private void Fall()
    {
        // NORMAL GRAVITY WHILE FALLING
        if (!_isGrounded && !_isJumping && !_isWallSliding && !_isWallJumping && !_isDashing && !_isDashFastFalling)
        {
            if (!_isFalling)
            {
                _isFalling = true;
            }

            VerticalVelocity += MoveStats.Gravity * Time.fixedDeltaTime;
        }
    }

    #endregion
    
    #region Jumping
    
    private void ResetJumpValues()
    {
        _isJumping = false;
        _isFastFalling = false;
        _isFalling = false;
        _fastFallTime = 0f;
        _isPastApexThreshold = false;
        // _numberOfJumpsUsed = 0;
    }
    
    private void JumpCheck()
    {
        // WHEN WE PRESS THE JUMP BUTTON
        if (inputManager.JumpWasPressed)
        {
            if (_isSwimming)
            {
                PerformSwimmingJump();
                return;
            }
            
            if (_isWallSlideFalling && _wallJumpPostBufferTimer >= 0f)
            {
                return;
            }
            
            else if (_isWallSliding || (_isTouchingWall && !_isGrounded))
            {
                return;
            }
            
            _jumpBufferTimer = MoveStats.JumpBufferTime;
            _jumpReleasedDuringBuffer = false;
        }

        // WHEN WE RELEASE THE JUMP BUTTON
        if (inputManager.JumpWasReleased)
        {
            if (_jumpBufferTimer > 0f)
            {
                _jumpReleasedDuringBuffer = true;
            }

            if (_isJumping && VerticalVelocity > 0f)
            {
                if (_isPastApexThreshold)
                {
                    _isPastApexThreshold = false;
                    _isFastFalling = true;
                    _fastFallTime = MoveStats.TimeForUpwardsCancel;
                    VerticalVelocity = 0f;
                }
                else
                {
                    _isFastFalling = true;
                    _fastFallReleaseSpeed = VerticalVelocity;
                }
            }
        }
        
        // INITIATE JUMP WITH JUMP BUFFERING AND COYOTE TIME
        if (_jumpBufferTimer > 0f && !_isJumping && (_isGrounded || _coyoteTimer > 0f))
        {
            InitiateJump(1);

            if (_jumpReleasedDuringBuffer)
            {
                _isFastFalling = true;
                _fastFallReleaseSpeed = VerticalVelocity;
            }
        }

        // DOUBLE JUMP
        else if (_jumpBufferTimer > 0f && (_isJumping || _isWallJumping || _isWallSlideFalling || _isAirDashing || _isDashFastFalling) 
                                       && !_isTouchingWall && _numberOfJumpsUsed < MoveStats.NumberOfJumpsAllowed)
        {
            _isFastFalling = false;
            InitiateJump(1);
            if (_isDashFastFalling)
            {
                _isDashFastFalling = false;
            }
        }


        // handle air jump AFTER the coyote time has lapsed (take off an extra jump so we don't get a bonus jump)
        else if (_jumpBufferTimer > 0f && _isFalling && !_isWallSlideFalling && _numberOfJumpsUsed < MoveStats.NumberOfJumpsAllowed - 1)
        {
            InitiateJump(2);
            _isFastFalling = false;
        }
    }
    
    private void InitiateJump(int numberOfJumpsUsed)
    {
        if (!_isJumping)
        {
            _isJumping = true;
        }
        
        ResetWallJumpValues();

        _jumpBufferTimer = 0f;
        _numberOfJumpsUsed += numberOfJumpsUsed;
        VerticalVelocity = MoveStats.InitialJumpVelocity;
    }
    
    private void PerformSwimmingJump()
    {
        VerticalVelocity = MoveStats.swimVerticalSpeed; // Use swimming-specific speed
        _isJumping = false; // Reset jump state immediately
        Debug.Log("Swimming Jump Performed");
    }

    
    private void Jump()
    {
    if (_isJumping)
    {
        // Swimming Gravity Adjustment
        if (_isSwimming)
        {
            VerticalVelocity -= MoveStats.waterGravityScale * Time.fixedDeltaTime;
        }
        else
        {
            // CHECK FOR HEAD BUMP
            if (_bumpedHead)
            {
                _isFastFalling = true;
            }

            // GRAVITY ON ASCENDING
            if (VerticalVelocity >= 0f)
            {
                _apexPoint = Mathf.InverseLerp(MoveStats.InitialJumpVelocity, 0f, VerticalVelocity);

                if (_apexPoint > MoveStats.ApexThreshold)
                {
                    if (!_isPastApexThreshold)
                    {
                        _isPastApexThreshold = true;
                        _timePastApexThreshold = 0f;
                    }

                    if (_isPastApexThreshold)
                    {
                        _timePastApexThreshold += Time.fixedDeltaTime;
                        if (_timePastApexThreshold < MoveStats.ApexHangTime)
                        {
                            VerticalVelocity = 0f;
                        }
                        else
                        {
                            VerticalVelocity = -0.01f;
                        }
                    }
                }
                else if (!_isFastFalling)
                {
                    VerticalVelocity += MoveStats.Gravity * Time.fixedDeltaTime;
                    if (_isPastApexThreshold)
                    {
                        _isPastApexThreshold = false;
                    }
                }
            }
            // GRAVITY ON DESCENDING
            else if (!_isFastFalling)
            {
                VerticalVelocity += MoveStats.Gravity * MoveStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
        }

        // FALLING STATE
        if (VerticalVelocity < 0f && !_isSwimming)
        {
            _isFalling = true;
        }
    }

    // JUMP CUT
    if (_isFastFalling)
    {
        if (_fastFallTime >= MoveStats.TimeForUpwardsCancel)
        {
            VerticalVelocity += MoveStats.Gravity * MoveStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
        }
        else if (_fastFallTime < MoveStats.TimeForUpwardsCancel)
        {
            VerticalVelocity = Mathf.Lerp(_fastFallReleaseSpeed, 0f, (_fastFallTime / MoveStats.TimeForUpwardsCancel));
        }

        _fastFallTime += Time.fixedDeltaTime;
    }
}

    
    #endregion
    
    #region Wall Slide
    
    private void WallSlideCheck()
    {
        if (_isTouchingWall && !_isGrounded && !_isDashing)
        {
            if (VerticalVelocity < 0f && !_isWallSliding)
            {
                ResetJumpValues();
                ResetWallJumpValues();
                ResetDashValues();
                
                if (MoveStats.ResetJumpsOnWallSlide)
                {
                    ResetDashes();
                }
                
                _isWallSlideFalling = false;
                _isWallSliding = true;

                if (MoveStats.ResetJumpsOnWallSlide)
                {
                    _numberOfJumpsUsed = 0;
                }
            }
        }
        else if (_isWallSliding && !_isTouchingWall && !_isGrounded && !_isWallSlideFalling)
        {
            _isWallSlideFalling = true;
            StopWallSlide();
        }
        else
        {
            StopWallSlide();
        }
    }

    private void StopWallSlide()
    {
        if (_isWallSliding)
        {
            _numberOfJumpsUsed++;
            _isWallSliding = false;
        }
    }

    private void WallSlide()
    {
        if (_isWallSliding)
        {
            VerticalVelocity = Mathf.Lerp(
                VerticalVelocity, 
                -MoveStats.WallSlideSpeed, 
                MoveStats.WallSlideDecelerationSpeed * Time.fixedDeltaTime
            );
        }
    }
    
    #endregion
    
    #region Wall Jump

    private void WallJumpCheck()
    {
        if (ShouldApllyPostWallJumpBuffer())
        {
            _wallJumpPostBufferTimer = MoveStats.WallJumpPostBufferTime;
        }

        // Wall jump fast falling
        if (inputManager.JumpWasReleased && !_isWallSliding && !_isTouchingWall && _isWallJumping)
        {
            if (VerticalVelocity > 0f)
            {
                if (_isPastWallJumpApexThreshold)
                {
                    _isPastWallJumpApexThreshold = false;
                    _isWallJumpFastFalling = true;
                    _wallJumpFastFallTime = MoveStats.TimeForUpwardsCancel;

                    VerticalVelocity = 0f;
                }
                else
                {
                    _isWallJumpFastFalling = true;
                    _wallJumpFastFallReleaseSpeed = VerticalVelocity;
                }
            }
        }

        if (inputManager.JumpWasPressed && _wallJumpPostBufferTimer > 0f)
        {
            InitiateWallJump();
        }
    }
    
    private void InitiateWallJump()
    {
        if (!_isWallJumping)
        {
            _isWallJumping = true;
            _useWallJumpMoveStats = true;
        }

        StopWallSlide();
        ResetJumpValues();
        _wallJumpTime = 0f;

        VerticalVelocity = MoveStats.InitialWallJumpVelocity;

        int dirMultiplier = 0;
        Vector2 hitPoint = _lastWallHit.collider.ClosestPoint(_bodyColl.bounds.center);

        if (hitPoint.x > transform.position.x)
        {
            dirMultiplier = -1;
        }
        else
        {
            dirMultiplier = 1;
        }

        horizontalVelocity = Mathf.Abs(MoveStats.WallJumpDirection.x) * dirMultiplier;
    }

    
    private void WallJump()
    {
        // APPLY WALL JUMP GRAVITY
        if (_isWallJumping)
        {
            // TIME TO TAKE OVER MOVEMENT CONTROLS WHILE WALL JUMPING
            _wallJumpTime += Time.fixedDeltaTime;
            if (_wallJumpTime >= MoveStats.TimeTillJumpApex)
            {
                _useWallJumpMoveStats = false;
            }

            // HIT HEAD
            if (_bumpedHead)
            {
                _isWallJumpFastFalling = true;
                _useWallJumpMoveStats = false;
            }

            // GRAVITY IN ASCENDING
            if (VerticalVelocity >= 0f)
            {
                // APEX CONTROLS
                _wallJumpApexPoint = Mathf.InverseLerp(MoveStats.InitialWallJumpVelocity, 0f, VerticalVelocity);

                if (_wallJumpApexPoint > MoveStats.ApexThreshold)
                {
                    if (!_isPastWallJumpApexThreshold)
                    {
                        _isPastWallJumpApexThreshold = true;
                        _timePastWallJumpApexThreshold = 0f;
                    }

                    if (_isPastWallJumpApexThreshold)
                    {
                        _timePastWallJumpApexThreshold += Time.fixedDeltaTime;
                        if (_timePastWallJumpApexThreshold < MoveStats.ApexHangTime)
                        {
                            VerticalVelocity = 0f;
                        }
                        else
                        {
                            VerticalVelocity = -0.01f;
                        }
                    }
                }
                // GRAVITY IN ASCENDING BUT NOT PAST APEX THRESHOLD
                else if (!_isWallJumpFastFalling)
                {
                    VerticalVelocity += MoveStats.WallJumpGravity * Time.fixedDeltaTime;
                    if (_isPastWallJumpApexThreshold)
                    {
                        _isPastWallJumpApexThreshold = false;
                    }
                }
            }
            // GRAVITY IN DESCENDING
            else if (!_isWallJumpFastFalling)
            {
                VerticalVelocity += MoveStats.WallJumpGravity  * Time.fixedDeltaTime;
            }
            else if(VerticalVelocity < 0f)
            {
                if (!_isWallJumpFalling)
                {
                    _isWallJumpFalling = true;
                }
            }
        }
        
        // HANDLE WALL JUMP CUT TIME
        if (_isWallJumpFastFalling)
        {
            if (_wallJumpFastFallTime >= MoveStats.TimeForUpwardsCancel)
            {
                VerticalVelocity += MoveStats.WallJumpGravity * MoveStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else if (_wallJumpFastFallTime < MoveStats.TimeForUpwardsCancel)
            {
                VerticalVelocity = Mathf.Lerp(_wallJumpFastFallReleaseSpeed, 0f, (_wallJumpFastFallTime / MoveStats.TimeForUpwardsCancel));
            }

            _wallJumpFastFallTime += Time.fixedDeltaTime;
        }
    }


    private bool ShouldApllyPostWallJumpBuffer()
    {
        if (!_isGrounded && (_isTouchingWall || _isWallSliding))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ResetWallJumpValues()
    {
        _isWallSlideFalling = false;
        _useWallJumpMoveStats = false;
        _isWallJumping = false;
        _isWallJumpFastFalling = false;
        _isWallJumpFalling = false;
        _isPastWallJumpApexThreshold = false;

        _wallJumpFastFallTime = 0f;
        _wallJumpTime = 0f;
    }

    #endregion

    #region Dash
    
    private void DashCheck()
    {
        if (inputManager.DashWasPressed)
        {
            if (_isGrounded && _dashOnGroundTimer < 0f && !_isDashing)
            {
                InitiateDash();
            }
            else if (!_isGrounded && !_isDashing && _numberOfDashesUsed < MoveStats.NumberOfDashes)
            {
                _isAirDashing = true;
                InitiateDash();
                
                // you left a wallslide but dashed within the wall jump post buffer timer
                if (_wallJumpPostBufferTimer > 0f)
                {
                    _numberOfDashesUsed--;
                    if (_numberOfDashesUsed < 0)
                    {
                        _numberOfDashesUsed = 0;
                    }
                }
            }
        }
    }
    
    private void InitiateDash()
    {
        _dashDirection = inputManager.Movement;

        Vector2 closestDirection = Vector2.zero;
        float minDistance = Vector2.Distance(_dashDirection, MoveStats.DashDirections[0]);

        for (int i = 0; i < MoveStats.DashDirections.Length; i++)
        {
            // Skip if we hit it bang on
            if (_dashDirection == MoveStats.DashDirections[i])
            {
                closestDirection = _dashDirection;
                break;
            }

            float distance = Vector2.Distance(_dashDirection, MoveStats.DashDirections[i]);

            // Check if this is a diagonal direction and apply bias
            bool isDiagonal = (Mathf.Abs(MoveStats.DashDirections[i].x) == 1 && 
                               Mathf.Abs(MoveStats.DashDirections[i].y) == 1);
            if (isDiagonal)
            {
                distance -= MoveStats.DashDiagonallyBias;
            }
            else if (distance < minDistance)
            {
                minDistance = distance;
                closestDirection = MoveStats.DashDirections[i];
            }
        }

        // Handle direction with NO input
        if (closestDirection == Vector2.zero)
        {
            if (_isFacingRight)
            {
                closestDirection = Vector2.right;
            }
            else
            {
                closestDirection = Vector2.left;
            }
        }
        
        _dashDirection = closestDirection;
        _isDashing = true;
        _dashTimer = 0f;
        _numberOfDashesUsed++;
        _dashOnGroundTimer = MoveStats.TimeBtwDashesOnGround;
        
        ResetJumpValues();
        ResetWallJumpValues();
        StopWallSlide();
    }
    
    private void Dash()
    {
        if (_isDashing)
        {
            // Stop the dash after the timer
            _dashTimer += Time.fixedDeltaTime;
            if (_dashTimer >= MoveStats.DashTime)
            {
                if (_isGrounded)
                {
                    ResetDashes();
                }

                _isAirDashing = false;
                _isDashing = false;

                if (!_isJumping && !_isWallJumping)
                {
                    _dashFastFallTime = 0f;
                    _dashFastFallReleaseSpeed = VerticalVelocity;

                    if (!_isGrounded)
                    {
                        _isDashFastFalling = true;
                    }
                }

                return;
            }

            horizontalVelocity = MoveStats.DashSpeed * _dashDirection.x;

            if (_dashDirection.y != 0f || _isAirDashing)
            {
                VerticalVelocity = MoveStats.DashSpeed * _dashDirection.y;
            }
        }
        
        // HANDLE DASH CUT TIME
        else if (_isDashFastFalling)
        {
            if (VerticalVelocity > 0f)
            {
                if (_dashFastFallTime < MoveStats.DashTimeForUpwardsCancel)
                {
                    VerticalVelocity = Mathf.Lerp(
                        _dashFastFallReleaseSpeed, 
                        0f, 
                        (_dashFastFallTime / MoveStats.DashTimeForUpwardsCancel)
                    );
                }
                else if (_dashFastFallTime >= MoveStats.DashTimeForUpwardsCancel)
                {
                    VerticalVelocity += MoveStats.Gravity * 
                                        MoveStats.DashGravityOnReleaseMultiplier * 
                                        Time.fixedDeltaTime;
                }

                _dashFastFallTime += Time.fixedDeltaTime;
            }
            else
            {
                VerticalVelocity += MoveStats.Gravity * 
                                    MoveStats.DashGravityOnReleaseMultiplier * 
                                    Time.fixedDeltaTime;
            }
        }
    }

    private void ResetDashValues()
    {
        _isDashFastFalling = false;
        _dashOnGroundTimer = -0.01f;
    }

    private void ResetDashes()
    {
        _numberOfDashesUsed = 0;
    }

    #endregion
    
    #region Collision Checks
    private void IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(_feetColl.bounds.center.x, _feetColl.bounds.min.y);
        Vector2 boxCastSize = new Vector2(_feetColl.bounds.size.x, MoveStats.groundDetectionRayLength);

        _groundHit = Physics2D.BoxCast(
            boxCastOrigin,
            boxCastSize,
            0f,
            Vector2.down,
            MoveStats.groundDetectionRayLength,
            MoveStats.groundLayer
        );

        if (_groundHit.collider != null)
        {
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
        }

        #region Debug Visualization
        if (MoveStats.DebugShowIsGroundedBox)
        {
            Color rayColor;
            if (_isGrounded)
            {
                rayColor = Color.green;
            }
            else
            {
                rayColor = Color.red;
            }
        
            Debug.DrawRay(
                new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y),
                Vector2.down * MoveStats.groundDetectionRayLength,
                rayColor
            );
            Debug.DrawRay(
                new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y),
                Vector2.down * MoveStats.groundDetectionRayLength,
                rayColor
            );
            Debug.DrawRay(
                new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - MoveStats.groundDetectionRayLength),
                Vector2.right * boxCastSize.x,
                rayColor
            );
        }
        #endregion
    }
    
    private void BumpedHead()
    {
        Vector2 boxCastOrigin = new Vector2(_feetColl.bounds.center.x, _bodyColl.bounds.max.y);
        Vector2 boxCastSize = new Vector2(_feetColl.bounds.size.x * MoveStats.headWidth, MoveStats.headDetectionRayLength);

        _headHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.up, MoveStats.headDetectionRayLength, MoveStats.groundLayer);

        if (_headHit.collider != null)
        {
            _bumpedHead = true;
        }
        else
        {
            _bumpedHead = false;
        }

        #region Debug Visualization

        if (MoveStats.DebugShowHeadBumpBox)
        {
            float headWidth = MoveStats.headWidth;
            Color rayColor;

            if (_bumpedHead)
            {
                rayColor = Color.green;
            }
            else
            {
                rayColor = Color.red;
            }

            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * headWidth, boxCastOrigin.y), Vector2.up * MoveStats.headDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + boxCastSize.x / 2 * headWidth, boxCastOrigin.y), Vector2.up * MoveStats.headDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * headWidth, boxCastOrigin.y + MoveStats.headDetectionRayLength), Vector2.right * boxCastSize.x * headWidth, rayColor);
        }

        #endregion
    }

    private void IsTouchingWall()
    {
        float originEndPoint = 0f;
        if (_isFacingRight)
        {
            originEndPoint = _bodyColl.bounds.max.x;
        }
        else
        {
            originEndPoint = _bodyColl.bounds.min.x;
        }

        float adjustedHeight = _bodyColl.bounds.size.y * MoveStats.wallDetectionRayHeightMultiplier;

        Vector2 boxCastOrigin = new Vector2(originEndPoint, _bodyColl.bounds.center.y);
        Vector2 boxCastSize = new Vector2(MoveStats.wallDetectionRayLength, adjustedHeight);

        _wallHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, transform.right, MoveStats.wallDetectionRayLength, MoveStats.groundLayer);

        if (_wallHit.collider != null)
        {
            _lastWallHit = _wallHit;
            _isTouchingWall = true;
        }
        else
        {
            _isTouchingWall = false;
        }

        #region Debug Visualization

        if (MoveStats.DebugShowWallHitBox)
        {
            Color rayColor;
            if (_isTouchingWall)
            {
                rayColor = Color.green;
            }
            else
            {
                rayColor = Color.red;
            }

            Vector2 boxBottomLeft = new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - boxCastSize.y / 2);
            Vector2 boxBottomRight = new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y - boxCastSize.y / 2);
            Vector2 boxTopLeft = new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y + boxCastSize.y / 2);
            Vector2 boxTopRight = new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y + boxCastSize.y / 2);

            Debug.DrawLine(boxBottomLeft, boxBottomRight, rayColor);
            Debug.DrawLine(boxBottomRight, boxTopRight, rayColor);
            Debug.DrawLine(boxTopRight, boxTopLeft, rayColor);
            Debug.DrawLine(boxTopLeft, boxBottomLeft, rayColor);
        }

        #endregion

    }
    
    private void CollisionChecks()
    {
        IsGrounded();
        BumpedHead();
        IsTouchingWall();
    }
    #endregion
    
    #region Timers
    private void CountTimers()
    {
        // JUMP BUFFER TIMER
        _jumpBufferTimer -= Time.deltaTime;
        
        // COYOTE TIMER
        if (!_isGrounded)
        {
            _coyoteTimer -= Time.deltaTime;
        }
        else
        {
            _coyoteTimer = MoveStats.JumpCoyoteTime;
        }
        
        // WALL JUMP BUFFER TIMER
        if (!ShouldApllyPostWallJumpBuffer())
        {
            _wallJumpPostBufferTimer -= Time.deltaTime;
        }
        
        // DASH TIMER
        if (_isGrounded)
        {
            _dashOnGroundTimer -= Time.deltaTime;
        }
    }
    #endregion
}