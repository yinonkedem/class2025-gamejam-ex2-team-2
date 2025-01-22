using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WallType
{
    LeftWall,
    RightWall,
    UpWall,
    BottomStones
}

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    private float _speed;

    [SerializeField]
    private float _rotationSpeed;

    [SerializeField]
    private float minSpeed = 1f;

    [SerializeField]
    private float maxSpeed = 10f;

    private Rigidbody2D _rigidbody;
    private PlayerAwarenessController _playerAwarenessController;
    private Vector2 _targetDirection;
    private float _changeDirectionCooldown;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerAwarenessController = GetComponent<PlayerAwarenessController>();
        _targetDirection = transform.up;
    }

    private void FixedUpdate()
    {
        UpdateTargetDirection();
        RotateTowardsTarget();
        SetVelocity();
    }

    private void UpdateTargetDirection()
    {
        HandleRandomDirectionChange();
        HandlePlayerTargeting();
    }

    private void HandleRandomDirectionChange()
    {
        _changeDirectionCooldown -= Time.deltaTime;

        if (_changeDirectionCooldown <= 0)
        {
            float angleChange = Random.Range(-45f, 45f); // Reduced angle range for smoother changes
            _targetDirection = Quaternion.Euler(0, 0, angleChange) * _targetDirection;
            _targetDirection.Normalize();

            _changeDirectionCooldown = Random.Range(1f, 5f);
        }
    }

    private void HandlePlayerTargeting()
    {
        if (_playerAwarenessController.AwareOfPlayer)
        {
            Vector2 directionToPlayer = _playerAwarenessController.DirectionToPlayer.normalized;
            _targetDirection = Vector2.Lerp(_targetDirection, directionToPlayer, Time.deltaTime * 2f);
            _targetDirection.Normalize();
        }
    }

    private void RotateTowardsTarget()
    {
        // Calculate the desired angle in degrees
        float desiredAngle = Mathf.Atan2(_targetDirection.y, _targetDirection.x) * Mathf.Rad2Deg - 90f;

        // Get the current rotation angle of the Rigidbody2D
        float currentAngle = _rigidbody.rotation;

        // Smoothly rotate towards the desired angle using MoveTowardsAngle
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, desiredAngle, _rotationSpeed * Time.deltaTime);

        // Apply the new rotation angle to the Rigidbody2D
        _rigidbody.MoveRotation(newAngle);
    }

    private void SetVelocity()
    {
        _rigidbody.velocity = _targetDirection * _speed;
    }

    private void HandleEnemyOffScreen(WallType wallType)
    {
        switch (wallType)
        {
            case WallType.LeftWall:
                if (_targetDirection.x > 0) return;
                _targetDirection = new Vector2(-_targetDirection.x, _targetDirection.y);
                break;
            case WallType.RightWall:
                if (_targetDirection.x < 0) return;
                _targetDirection = new Vector2(-_targetDirection.x, _targetDirection.y);
                break;
            case WallType.BottomStones:
                if (_targetDirection.y > 0) return;
                _targetDirection = new Vector2(_targetDirection.x, -_targetDirection.y);
                break;
            case WallType.UpWall:
                if (_targetDirection.y < 0) return;
                _targetDirection = new Vector2(_targetDirection.x, -_targetDirection.y);
                break;
        }

        _targetDirection.Normalize();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        WallType? wallType = GetWallTypeFromTag(collision.tag);
        if (wallType.HasValue)
        {
            HandleEnemyOffScreen(wallType.Value);
        }
    }

    private WallType? GetWallTypeFromTag(string tag)
    {
        switch (tag)
        {
            case "Left Wall":
                return WallType.LeftWall;
            case "Right Wall":
                return WallType.RightWall;
            case "Up Wall":
                return WallType.UpWall;
            case "Bottom Stones":
                return WallType.BottomStones;
            default:
                return null;
        }
    }

    // Add to speed function with clamping
    public void AddToSpeed(float speedToAdd)
    {
        _speed = Mathf.Clamp(_speed + speedToAdd, minSpeed, maxSpeed);
    }

    // Debugging aid
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)_targetDirection * 2f);
    }
}
