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
    [SerializeField] private float oxygenDecreasedNumberFromBoltAttack = 5f;
    [SerializeField] private float oxygenTransferRate = 3f;
    [SerializeField] GameObject otherPlayer;
    [SerializeField] private float oxygenDecreasedNumberFromMiniEnemyExplosion = 7f;
    [SerializeField] private float oxygenDecreasedNumberFromEnemyCollision = 3f;
    private BarController oxygenBarController;
    private bool isTouchingWaterEnding = false;
    private bool isTouchingPlayer = false;
    private GameObject currentAttack; 
    private float currentOxygenValue;
    
    private InputManager inputManager;
    
    // Start is called before the first frame update
    void Start()
    {
        currentOxygenValue = maxTimeWithoutOxygen;
        oxygenBarController = oxygenBar.GetComponent<BarController>();
        oxygenBarController.updateBar(currentOxygenValue,maxTimeWithoutOxygen);
        InvokeRepeating("UpdateOxygen", 0f, 1f); 
        EventManager.Instance.StartListening(EventManager.EVENT_DECREASE_PLAYER_LIFE,DecreasePlayerLifeAfterTouchMiniEnemyExplosion );

    }

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateOxygenBarPosition();
        if (inputManager.AttackWasPressed)
        {
            Attack();
        }
        if (inputManager.Oxygen)
        {
            if (isTouchingPlayer && otherPlayer != null)
            {
                PassOxygen();
            }
            
        }
    }
    private void DecreasePlayerLifeAfterTouchMiniEnemyExplosion(GameObject data)
    {
        currentOxygenValue -= oxygenDecreasedNumberFromMiniEnemyExplosion;
        oxygenBarController.updateBar(currentOxygenValue,maxTimeWithoutOxygen);
    }
    
    private void PassOxygen()
    {
        if (currentOxygenValue - oxygenTransferRate >= 0)
        {
            currentOxygenValue -= oxygenTransferRate;
            PlayerController otherPlayerController = otherPlayer.GetComponent<PlayerController>();
            if (otherPlayerController != null)
            {
                otherPlayerController.AddToOxygen(oxygenTransferRate);
            }
        }
    }
    
    public void AddToOxygen(float amount)
    {
        if(currentOxygenValue + amount > maxTimeWithoutOxygen)
        {
            currentOxygenValue = maxTimeWithoutOxygen;
        }
        else
        {
            currentOxygenValue += amount;
        }
        oxygenBarController.updateBar(currentOxygenValue,maxTimeWithoutOxygen);
    }

    private void Attack()
    {
        AudioController.Instance.PlayShooting();
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
        if (collider.CompareTag("Water Ending"))
        {
            if (currentOxygenValue <= maxTimeWithoutOxygen)
            {
                AudioController.Instance.PlayOxygenIncrease();
                currentOxygenValue += oxygenAddedAfterSecondInTheAir;
                oxygenBarController.updateBar(currentOxygenValue, maxTimeWithoutOxygen);
            }
        }

        if (collider.CompareTag("Player"))
        {
            isTouchingPlayer = true;
        }
        
        
        if (collider.CompareTag("Enemy"))
        {
            Debug.Log("Player is hit by enemy");
            AudioController.Instance.PlayOxygenDecrease();
            currentOxygenValue -= oxygenDecreasedNumberFromEnemyCollision;
            oxygenBarController.updateBar(currentOxygenValue,maxTimeWithoutOxygen);
        }
        
    }
    
    
    private void UpdateOxygenBarPosition()
    {
        // Set the oxygen bar's position relative to the player
        Vector3 oxygenBarPosition = transform.position; // Get the player's position
        oxygenBarPosition.y += 1f;  // Offset to place it above the player
        oxygenBar.transform.position = oxygenBarPosition;  // Update oxygen bar's position
    }
    
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bolt"))
        {
            Debug.Log("Player is hit by bolt attack");
            AudioController.Instance.PlayOxygenDecrease();
            currentOxygenValue -= oxygenDecreasedNumberFromBoltAttack;
            oxygenBarController.updateBar(currentOxygenValue,maxTimeWithoutOxygen);
        }
        if(other.CompareTag("Ink"))
        {
            Debug.Log("Player is hit by ink attack");
            AudioController.Instance.PlayOxygenDecrease();
            currentOxygenValue -= oxygenDecreasedNumberFromInkCollision;
            oxygenBarController.updateBar(currentOxygenValue,maxTimeWithoutOxygen);
        }

        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Player is hit by enemy");
            AudioController.Instance.PlayOxygenDecrease();
            currentOxygenValue -= oxygenDecreasedNumberFromEnemyCollision;
            oxygenBarController.updateBar(currentOxygenValue,maxTimeWithoutOxygen);
        }
        
        if(other.CompareTag("Water Ending"))
        {
            isTouchingWaterEnding = true;
        }

    }
    
   


    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isTouchingPlayer = false;
        }
        if(other.CompareTag("Water Ending"))
        {
            isTouchingWaterEnding = false;
        }
    }
    
    

    private void UpdateOxygen()
    {
        if (currentOxygenValue <= 0 && !GameManager.Instance.ArePlayerWon)
        {
            Die();
        }
        if (currentOxygenValue > 0&& !isTouchingWaterEnding)
        {
            currentOxygenValue -= 1f;
        }

        oxygenBarController.updateBar(currentOxygenValue,maxTimeWithoutOxygen);

    }

    private void Die()
    {
        Debug.Log("Player is dead");
        Destroy(gameObject);
        Destroy(oxygenBar);
    }


}
