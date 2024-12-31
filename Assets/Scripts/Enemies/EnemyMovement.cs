////using System.Collections;
////using System.Collections.Generic;
////using UnityEngine;

////public class EnemyMovement : MonoBehaviour
////{
////    [SerializeField]
////    private float _speed;

////    [SerializeField]
////    private float _rotationSpeed;

////    private Rigidbody2D _rigidbody;
////    private PlayerAwarenessController _playerAwarenessController;
////    private Vector2 _targetDirection;

////    private void Awake()
////    {
////        _rigidbody = GetComponent<Rigidbody2D>();
////        _playerAwarenessController = GetComponent<PlayerAwarenessController>();
////    }

////    private void FixedUpdate()
////    {
////        UpdateTargetDirection();
////        RotateTowardsTarget();
////        SetVelocity();
////    }

////    private void UpdateTargetDirection()
////    {
////        if (_playerAwarenessController.AwareOfPlayer)
////        {
////            _targetDirection = _playerAwarenessController.DirectionToPlayer;
////        }
////        else
////        {
////            _targetDirection = Vector2.zero;
////        }
////    }

////    private void RotateTowardsTarget()
////    {
////        if (_targetDirection == Vector2.zero)
////        {
////            return;
////        }

////        Quaternion targetRotation = Quaternion.LookRotation(transform.forward, _targetDirection);
////        Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

////        _rigidbody.SetRotation(rotation);
////    }

////    private void SetVelocity()
////    {
////        if (_targetDirection == Vector2.zero)
////        {
////            _rigidbody.velocity = Vector2.zero;
////        }
////        else
////        {
////            _rigidbody.velocity = transform.up * _speed;
////        }
////    }
////}














//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class EnemyMovement : MonoBehaviour
//{
//    [SerializeField]
//    private float _speed;

//    [SerializeField]
//    private float _rotationSpeed;

//    private Rigidbody2D _rigidbody;
//    private PlayerAwarenessController _playerAwarenessController;
//    private Vector2 _targetDirection;
//    private float _changeDirectionCooldown;

//    private void Awake()
//    {
//        _rigidbody = GetComponent<Rigidbody2D>();
//        _playerAwarenessController = GetComponent<PlayerAwarenessController>();
//        _targetDirection = transform.up;
//    }

//    private void FixedUpdate()
//    {
//        UpdateTargetDirection();
//        RotateTowardsTarget();
//        SetVelocity();
//    }

//    private void UpdateTargetDirection()
//    {
//        HandleRandomDirectionChange();
//        HandlePlayerTargeting();
//    }

//    private void HandleRandomDirectionChange()
//    {
//        _changeDirectionCooldown -= Time.deltaTime;

//        if (_changeDirectionCooldown <= 0)
//        {
//            float angleChange = Random.Range(-90f, 90f);
//            Quaternion rotation = Quaternion.AngleAxis(angleChange, transform.forward);
//            _targetDirection = rotation * _targetDirection;

//            _changeDirectionCooldown = Random.Range(1f, 5f);
//        }
//    }

//    private void HandlePlayerTargeting()
//    {
//        if (_playerAwarenessController.AwareOfPlayer)
//        {
//            _targetDirection = _playerAwarenessController.DirectionToPlayer;
//        }
//    }

//    private void RotateTowardsTarget()
//    {
//        Quaternion targetRotation = Quaternion.LookRotation(transform.forward, _targetDirection);
//        Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

//        _rigidbody.SetRotation(rotation);
//    }

//    private void SetVelocity()
//    {
//        _rigidbody.velocity = transform.up * _speed;
//    }



//}





















//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class EnemyMovement : MonoBehaviour
//{
//    [SerializeField]
//    private float _speed;

//    [SerializeField]
//    private float _rotationSpeed;

//    private Rigidbody2D _rigidbody;
//    private PlayerAwarenessController _playerAwarenessController;
//    private Vector2 _targetDirection;

//    private void Awake()
//    {
//        _rigidbody = GetComponent<Rigidbody2D>();
//        _playerAwarenessController = GetComponent<PlayerAwarenessController>();
//    }

//    private void FixedUpdate()
//    {
//        UpdateTargetDirection();
//        RotateTowardsTarget();
//        SetVelocity();
//    }

//    private void UpdateTargetDirection()
//    {
//        if (_playerAwarenessController.AwareOfPlayer)
//        {
//            _targetDirection = _playerAwarenessController.DirectionToPlayer;
//        }
//        else
//        {
//            _targetDirection = Vector2.zero;
//        }
//    }

//    private void RotateTowardsTarget()
//    {
//        if (_targetDirection == Vector2.zero)
//        {
//            return;
//        }

//        Quaternion targetRotation = Quaternion.LookRotation(transform.forward, _targetDirection);
//        Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

//        _rigidbody.SetRotation(rotation);
//    }

//    private void SetVelocity()
//    {
//        if (_targetDirection == Vector2.zero)
//        {
//            _rigidbody.velocity = Vector2.zero;
//        }
//        else
//        {
//            _rigidbody.velocity = transform.up * _speed;
//        }
//    }
//}














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

    }





}






