using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject attackPrefab;
    [SerializeField] private float maxTimeWithoutOxygen = 30f;
    [SerializeField] private float oxygenAddedAfterSecondInTheAir = 3f;
    [SerializeField] private float oxygenDecreasedNumberFromInkCollision = 5f;
    [SerializeField] private float oxygenDecreasedNumberWhenShooting = 1f;
    [SerializeField] private GameObject oxygenBar;
    [SerializeField] private float attackSpeed = 10f;
    [SerializeField] private KeyCode attackKeyCode = KeyCode.End;
    [SerializeField] private float oxygenDecreasedNumberFromBoltAttack = 5f;

    private BarController oxygenBarController;
    private bool isTouchingOxygenGroup = false;


    private GameObject currentAttack; 
    private float currentOxygenValue;
    
    // Start is called before the first frame update
    void Start()
    {
        currentOxygenValue = maxTimeWithoutOxygen;
        oxygenBarController = oxygenBar.GetComponent<BarController>();
        oxygenBarController.updateBar(currentOxygenValue,maxTimeWithoutOxygen);
        InvokeRepeating("UpdateOxygen", 0f, 1f);  
    }

    // Update is called once per frame
    void Update()
    {
        UpdateOxygenBarPosition();
        if (Input.GetKeyDown(attackKeyCode))
        {
            Attack();
        }
    }

    private void Attack()
    {
        Vector3 attackPosition = transform.position + new Vector3(1f, 0f, 0f); // Position it slightly in front of the player
        GameObject attackObject = Instantiate(attackPrefab, attackPosition, Quaternion.identity, GameObject.Find("Main").transform);

        // Add velocity to the attack
        Rigidbody2D rb = attackObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = new Vector2(0f, attackSpeed); // Adjust the speed and direction as needed
        }
        
        currentOxygenValue -= oxygenDecreasedNumberWhenShooting;
        oxygenBarController.updateBar(currentOxygenValue,maxTimeWithoutOxygen);

        Debug.Log("PinkPlayer attacked!");
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.CompareTag("Oxygen grounp"))
        {
            if (currentOxygenValue <= maxTimeWithoutOxygen)
            {
                currentOxygenValue += oxygenAddedAfterSecondInTheAir;
                oxygenBarController.updateBar(currentOxygenValue, maxTimeWithoutOxygen);
            }
        }
    }
    
    
    private void UpdateOxygenBarPosition()
    {
        // Set the oxygen bar's position relative to the player
        Vector3 oxygenBarPosition = transform.position; // Get the player's position
        oxygenBarPosition.y += 1f;  // Offset to place it above the player
        oxygenBar.transform.position = oxygenBarPosition;  // Update oxygen bar's position
    }
    

    private bool IsAboveObjectWithTag(string tag)
    {
        Vector2 playerCenter = GetComponent<Collider2D>().bounds.center;

        // Start the ray slightly below the player's center for detection purposes
        Vector2 rayOrigin = new Vector2(playerCenter.x, playerCenter.y - 1f);

        // Draw the ray in the scene for debugging (only for visualization purposes)
        Debug.DrawRay(rayOrigin, Vector2.down * 5f, Color.red, 1f);

        // Cast the ray downward from just below the player's center
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity);

        // Check if a collider was hit
        if (hit.collider != null)
        {
            // Check if the object hit by the ray has the correct tag
            if (hit.collider.CompareTag(tag))
            {
                Debug.Log("Player is above the water!");
                return true;
            }
        }

        return false;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bolt"))
        {
            Debug.Log("Player is hit by bolt attack");
            currentOxygenValue -= oxygenDecreasedNumberFromBoltAttack;
            oxygenBarController.updateBar(currentOxygenValue,maxTimeWithoutOxygen);
        }
        if(other.CompareTag("Ink"))
        {
            Debug.Log("Player is hit by ink attack");
            currentOxygenValue -= oxygenDecreasedNumberFromInkCollision;
            oxygenBarController.updateBar(currentOxygenValue,maxTimeWithoutOxygen);
        }
        if (other.CompareTag("Oxygen grounp"))
        {
            isTouchingOxygenGroup = true;
        }

    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Oxygen grounp"))
        {
            isTouchingOxygenGroup = false;
        }
    }
    
    

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Oxygen grounp"))
        {
            Debug.Log("Player is above the water!");
            currentOxygenValue += oxygenAddedAfterSecondInTheAir;
            oxygenBarController.updateBar(currentOxygenValue,maxTimeWithoutOxygen);
        }
    }
    

    private void UpdateOxygen()
    {
        if (currentOxygenValue <= 0 && !GameManager.Instance.ArePlayerWon)
        {
            Die();
        }
        if (currentOxygenValue > 0&& !isTouchingOxygenGroup)
        {
            // if player is not touching gameobject with tag "Oxygen grounp" decrease the oxygen value by 1
            
            currentOxygenValue -= 1f;
            Debug.Log("current oxygen value is: " + currentOxygenValue);
        }

        oxygenBarController.updateBar(currentOxygenValue,maxTimeWithoutOxygen);

    }

    private void Die()
    {
        GameManager.Instance.ArePlayersDefeated = true;
        Debug.Log("Player is dead");
        Destroy(gameObject);
        Destroy(oxygenBar);
    }


}
