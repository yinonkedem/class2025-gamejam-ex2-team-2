using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    private float _speed;

    [SerializeField]
    private float _rotationSpeed;

    private Rigidbody2D _rigidbody;
    private PlayerAwarenessController _playerAwarenessController;
    private Vector2 _targetDirection;
    private float _changeDirectionCooldown;
    private BoxCollider2D _boxCollider;
    private bool isAllowoedToMove = true;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerAwarenessController = GetComponent<PlayerAwarenessController>();
        _targetDirection = transform.up;
        _boxCollider = GetComponent<BoxCollider2D>();

    }

    private void FixedUpdate()
    {
        UpdateTargetDirection();
        RotateTowardsTarget();
        SetVelocity();
    }

    private void UpdateTargetDirection()
    {
        if(!isAllowoedToMove)
        {
            return;
        }
        HandleRandomDirectionChange();
        HandlePlayerTargeting();
    }

    private void HandleRandomDirectionChange()
    {
        _changeDirectionCooldown -= Time.deltaTime;

        if (_changeDirectionCooldown <= 0)
        {
            float angleChange = Random.Range(-90f, 90f);
            Quaternion rotation = Quaternion.AngleAxis(angleChange, transform.forward);
            _targetDirection = rotation * _targetDirection;

            _changeDirectionCooldown = Random.Range(1f, 5f);
        }
    }

    private void HandlePlayerTargeting()
    {
        if (_playerAwarenessController.AwareOfPlayer)
        {
            _targetDirection = _playerAwarenessController.DirectionToPlayer;
        }
    }

    private void RotateTowardsTarget()
    {
        Quaternion targetRotation = Quaternion.LookRotation(transform.forward, _targetDirection);
        Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

        _rigidbody.SetRotation(rotation);
    }

    private void SetVelocity()
    {
        _rigidbody.velocity = transform.up * _speed;
    }

    private void HandleEnemyOffScreen(string wallKind)
    {

        if (wallKind.Equals("Left Wall") || wallKind.Equals("Right Wall"))
        {
            if (wallKind.Equals("Left Wall"))
            {
                if (_targetDirection.x > 0)
                {
                    return;
                }
            }
            else
            {
                if (_targetDirection.x < 0)
                {
                    return;
                }
            }
            _targetDirection = new Vector2(-_targetDirection.x, _targetDirection.y);
        }

        if (wallKind.Equals("Bottom Wall") || wallKind.Equals("Up Wall"))
        {
            if(wallKind.Equals("Bottom Wall"))
            {
                if(_targetDirection.y>0)
                {
                    return;
                }
            }
            else
            {
                if (_targetDirection.y < 0)
                {
                    return;
                }
            }
            _targetDirection = new Vector2(_targetDirection.x, -_targetDirection.y);
        }
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleEnemyOffScreen(collision.gameObject.tag);
        if (collision.gameObject.tag.EndsWith("Player"))
        {
            if (collision.gameObject.tag.Equals("Grey Player"))
            {
                EventManager.Instance.TriggerEvent(EventManager.EVENT_GREY_PLAYER_DIE, gameObject);
            }
            else
            {
                EventManager.Instance.TriggerEvent(EventManager.EVENT_PINK_PLAYER_DIE, gameObject);
            }
        }

    }

    //function that stop the movement for X seconds
    public IEnumerator StopMovement(float time)
    {
        float originSpeed = _speed;
        _speed = 0;
        isAllowoedToMove = false;
        yield return new WaitForSeconds(time);
        _speed = originSpeed;
        isAllowoedToMove = true;
    }




}






