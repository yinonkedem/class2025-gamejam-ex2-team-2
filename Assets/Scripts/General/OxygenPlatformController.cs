// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// using UnityEngine;
//
// public class OxygenPlatformController : MonoBehaviour
// {
//     [SerializeField] private float moveSpeed = 2f; // Speed of movement
//     private float leftLimit; // Left wall position
//     private float rightLimit; // Right wall position
//     private bool isPlayerOnThePlatform = false;
//     
//     private void Start()
//     {
//         // Initialize wall positions from GameManager
//         leftLimit = GameManager.Instance.LeftWallPosition + transform.localScale.x;
//         rightLimit = GameManager.Instance.RightWallPosition - transform.localScale.x;
//     }
//
//     private void OnCollisionStay2D(Collision2D other)
//     {
//         if(other.gameObject.CompareTag("Player"))
//         {
//             isPlayerOnThePlatform = true;
//         }
//     }
//     
//     private void OnCollisionEnter2D(Collision2D other)
//     {
//         if(other.gameObject.CompareTag("Player"))
//         {
//             isPlayerOnThePlatform = true;
//         }
//     }
//     
//     private void OnCollisionExit2D(Collision2D other)
//     {
//         if(other.gameObject.CompareTag("Player"))
//         {
//             isPlayerOnThePlatform = false;
//         }
//     }
//
//     private void Update()
//     {
//         if (!isPlayerOnThePlatform)
//         {
//             // Calculate new x position using PingPong for smooth back-and-forth movement
//             float newX = Mathf.PingPong(Time.time * moveSpeed, rightLimit - leftLimit) + leftLimit;
//
//             // Update the object's position
//             transform.position = new Vector3(newX, transform.position.y, transform.position.z);
//         }
//
//     }
// }





using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenGroundController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f; // Speed of movement
    private float leftLimit; // Left wall position
    private float rightLimit; // Right wall position
    private bool isPlayerOnThePlatform = false;
    private float elapsedTime = 0f; // Time tracker for PingPong movement

    private void Start()
    {
        // Initialize wall positions from GameManager
        leftLimit = GameManager.Instance.LeftWallPosition + transform.localScale.x/2;
        rightLimit = GameManager.Instance.RightWallPosition - transform.localScale.x/2;
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerOnThePlatform = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerOnThePlatform = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerOnThePlatform = false;
        }
    }

    private void Update()
    {
        if (!isPlayerOnThePlatform)
        {
            // Increment elapsedTime only when the platform is moving
            elapsedTime += Time.deltaTime;

            // Calculate new x position using PingPong for smooth back-and-forth movement
            float newX = Mathf.PingPong(elapsedTime * moveSpeed, rightLimit - leftLimit) + leftLimit;

            // Update the object's position
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }
    }
}
