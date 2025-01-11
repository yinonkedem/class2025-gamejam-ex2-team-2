using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreyPlayer : MonoBehaviour
{
    [SerializeField] private GameObject greyAttackPrefab;
    [SerializeField] private float maxTimeWithoutOxygen = 30f;
    [SerializeField] private float oxygenAddedAfterSecondInTheAir = 3f;
    [SerializeField] private GameObject oxygenBar;
    [SerializeField] private float attackSpeed = 10f;
    [SerializeField] private KeyCode attackKeyCode = KeyCode.End;
    [SerializeField] private float oxygenDecreasedNumberFromBoltAttack = 5f;

    private OxygenBarController oxygenBarController;


    private GameObject currentAttack; 
    private bool isUnderAttack = false;
    private bool isAttacking = false;
    private bool isHaveAnAttack = false;
    private bool isAboveWater = false;
    private float currentOxygenValue;
    
    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.StartListening(EventManager.EVENT_GREY_PLAYER_HIT_FROM_ATTACK, HitFromAttack);
        EventManager.Instance.StartListening(EventManager.EVENT_GREY_PLAYER_DIE, Die);
        currentOxygenValue = maxTimeWithoutOxygen;
        oxygenBarController = oxygenBar.GetComponent<OxygenBarController>();
        oxygenBarController.updateBar(currentOxygenValue,maxTimeWithoutOxygen);
        InvokeRepeating("UpdateOxygen", 0f, 1f);  
    }

    // Update is called once per frame
    void Update()
    {
        if (IsAboveObjectWithTag("Water Ending"))
        {
            Debug.Log("Player is above the water!");
            isAboveWater = true;
        }else
        {
            isAboveWater = false;
        }
        UpdateOxygenBarPosition();
        if (Input.GetKeyDown(attackKeyCode))
        {
            Debug.Log("GreyPlayer pressed E to attack!");
            Attack();
        }
    }

    private void Attack()
    {
        Vector3 attackPosition = transform.position + new Vector3(1f, 0f, 0f); // Position it slightly in front of the player
        GameObject attackObject = Instantiate(greyAttackPrefab, attackPosition, Quaternion.identity);

        // Add velocity to the attack
        Rigidbody2D rb = attackObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = new Vector2(0f, attackSpeed); // Adjust the speed and direction as needed
        }

        Debug.Log("PinkPlayer attacked!");
    }
    private void HitFromAttack(GameObject obj)
    {
        //TODO : start animation
        isUnderAttack = true;
    }
    

    private void Die(GameObject arg0)
    {
        Debug.Log("Grey player is dead"); 
        DieLogic();
    }

    private void DieLogic()
    {
        
    }
    
    
    private void UpdateOxygenBarPosition()
    {
        if (oxygenBar != null)
        {
            // Set the oxygen bar's position relative to the player
            Vector3 oxygenBarPosition = transform.position; // Get the player's position
            oxygenBarPosition.y += 1f;  // Offset to place it above the player
            oxygenBar.transform.position = oxygenBarPosition;  // Update oxygen bar's position
        }
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
            Debug.Log("Grey player is hit by bolt attack");
            currentOxygenValue -= oxygenDecreasedNumberFromBoltAttack;
            oxygenBarController.updateBar(currentOxygenValue,maxTimeWithoutOxygen);
        }
    }

    private void UpdateOxygen()
    {
        if (currentOxygenValue <= 0)
        {
            DieLogic();
        }
        if (!isAboveWater && currentOxygenValue > 0)
        {
            currentOxygenValue -= 1f;
        }
        else if (isAboveWater)
        {
            currentOxygenValue += oxygenAddedAfterSecondInTheAir;
        }
        oxygenBarController.updateBar(currentOxygenValue,maxTimeWithoutOxygen);

    }
}
