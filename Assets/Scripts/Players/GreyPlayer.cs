using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreyPlayer : MonoBehaviour
{
    [SerializeField] private GameObject greyAttackPrefab;
    [SerializeField] private KeyCode attackKey;
    [SerializeField] private float maxTimeWithoutOxygen = 30f;
    [SerializeField] private float oxygenAddedAfterSecondInTheAir = 3f;
    [SerializeField] private GameObject oxygenBar;
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
        if (Input.GetKey(attackKey) && isHaveAnAttack)
        {
            Attack(gameObject);
        }
        if (IsAboveObjectWithTag("Water Ending"))
        {
            Debug.Log("Player is above the water!");
            isAboveWater = true;
        }else
        {
            isAboveWater = false;
        }
        UpdateOxygenBarPosition();
    }

    private void Attack(GameObject obj)
    {

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
            Debug.Log($"Raycast hit: {hit.collider.name}");

            // Check if the object hit by the ray has the correct tag
            if (hit.collider.CompareTag(tag))
            {
                Debug.Log("Player is above the water!");
                return true;
            }
        }

        return false;
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
