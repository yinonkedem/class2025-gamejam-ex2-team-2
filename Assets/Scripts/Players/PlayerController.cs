using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject attackPrefab;
    [SerializeField] private GameObject otherPlayerPrefab;
    [SerializeField] private GameObject otherPlayerOxygenBar;
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
    [SerializeField] private int numberOfTimesPlayerCanBackToLife = 2;
    [SerializeField] private Sprite playerDeadSprite1;
    [SerializeField] private Sprite playerDeadSprite2;
    [SerializeField] private Sprite playerDeadSprite3;
    [SerializeField]private float attackCooldown = 0.4f;
    [SerializeField] private GameObject airPrefab; // Assign the "Air" prefab in the inspector
    [SerializeField] private float airSpeed = 5f;
    [SerializeField] private int playerType;
    
    private BarController oxygenBarController;
    private bool isTouchingWaterEnding = false;
    private bool isTouchingPlayer = false;
    private bool isTouchingDeadPlayer = false;
    private GameObject currentAttack; 
    private float currentOxygenValue;
    private Animator _animator;
    private bool isDead = false;
    private InputManager inputManager;
    private int i = 1;
    private float lastAttackTime = -Mathf.Infinity; // Initialize to a very low value
    private bool sendAir = false;

    
    // Start is called before the first frame update
    void Start()
    {
        oxygenBarController = oxygenBar.GetComponent<BarController>();
        oxygenBarController.updateBar(currentOxygenValue,maxTimeWithoutOxygen);
        InvokeRepeating("UpdateOxygen", 0f, 1f); 
        EventManager.Instance.StartListening(EventManager.EVENT_DECREASE_PLAYER_LIFE,DecreasePlayerLifeAfterTouchMiniEnemyExplosion );
        _animator = GetComponent<Animator>();
        isDead = false;
    }

    private void Awake()
    {
        currentOxygenValue = maxTimeWithoutOxygen;
        inputManager = GetComponent<InputManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (oxygenBar != null)
        {
            UpdateOxygenBarPosition();
        }
        if (inputManager.AttackWasPressed && Time.time >= lastAttackTime + attackCooldown)
        {
            StartCoroutine(Attack());
            lastAttackTime = Time.time; // Update the last attack time
        }
        if (inputManager.Oxygen)
        {
            if(otherPlayer != null && isTouchingWaterEnding)
            {
                sendAir = true;
                PassOxygen();
            }

            if(isTouchingDeadPlayer)
            { 
                ReturnOtherPlayerToLife();
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
                SpawnAndMoveAir();
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
    

    private IEnumerator Attack()
    {
        if (currentOxygenValue > 0)
        {
            _animator.SetBool("isShoot", true);
            AudioController.Instance.PlayShooting();
            Vector3 attackPosition = transform.position + new Vector3(0f, 1f, 0f); // Position it slightly in front of the player
            GameObject attackObject = Instantiate(attackPrefab, attackPosition, Quaternion.identity, GameObject.Find("Main").transform);

            // Add velocity to the attack
            Rigidbody2D rb = attackObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = new Vector2(0f, attackSpeed); // Adjust the speed and direction as needed
            }
        
            currentOxygenValue -= oxygenDecreasedNumberWhenShooting;
            oxygenBarController.updateBar(currentOxygenValue,maxTimeWithoutOxygen);
            yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
            _animator.SetBool("isShoot", false);
            Debug.Log("Player attacked!");
        }
    }
    

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.CompareTag("Water Ending"))
        {
            if (currentOxygenValue <= maxTimeWithoutOxygen)
            {
                i++;
                currentOxygenValue += oxygenAddedAfterSecondInTheAir;
                oxygenBarController.updateBar(currentOxygenValue, maxTimeWithoutOxygen);
            }
        }

        if (collider.CompareTag("Player"))
        {
            isTouchingPlayer = true;
        }

        if (collider.CompareTag("Dead Player"))
        {
            isTouchingDeadPlayer = true;
        }
        if (collider.CompareTag("Enemy"))
        {
            Debug.Log("Player is hit by enemy");
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
            AudioController.Instance.PlayIncreaseOxygen();
        }

        if (other.CompareTag("Air"))
        {
            if (!sendAir)
            {
                AudioController.Instance.PlayIncreaseOxygen();
                AddToOxygen(oxygenTransferRate);    
            }
        }

    }
    
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isTouchingPlayer = false;
        }
        if (other.CompareTag("Dead Player"))
        {
            isTouchingDeadPlayer = false;
        }
        if(other.CompareTag("Water Ending"))
        {
            isTouchingWaterEnding = false;
        }
    }

    private void TurnToDeadPlayer()
    {
        GameManager.Instance.NumOfPlayersDeadUntilNow++;
        GameManager.Instance.NumOfPlayersDead++;
        GameObject playerDead = Utils.Instance.FindInactiveObjectByName("Player Dead");
        Sprite newSprite;
        if (GameManager.Instance.NumOfPlayersDeadUntilNow == 1)
        {
            newSprite = playerDeadSprite1;
        }
        else
        {
             newSprite = playerDeadSprite2; 
        }
     
        SpriteRenderer spriteRenderer = playerDead.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = newSprite;
        playerDead.SetActive(true);

        StartCoroutine(Die());
        oxygenBar.SetActive(false);
        
    }
    
    
    private void SpawnAndMoveAir()
    {
        // Instantiate the "Air" prefab at the current player's position
        Vector3 spawnPosition = transform.position;
        GameObject airObject = Instantiate(airPrefab, spawnPosition, Quaternion.identity);

        // Move the "Air" toward the other player
        StartCoroutine(MoveAirToOtherPlayer(airObject));
    }

    private IEnumerator MoveAirToOtherPlayer(GameObject airObject)
    {
        Vector3 startPosition = airObject.transform.position;
        Vector3 targetPosition = otherPlayer.transform.position;

        float journey = 0f; 
        while (journey < 1f)
        {
            journey += Time.deltaTime * airSpeed / Vector3.Distance(startPosition, targetPosition);
            airObject.transform.position = Vector3.Lerp(startPosition, targetPosition, journey);

            yield return null; 
        }

        airObject.transform.position = targetPosition;

        Destroy(airObject);
        sendAir = false;
    }
    

    private void UpdateOxygen()
    {
        if (!isDead && currentOxygenValue <= 0 && !GameManager.Instance.ArePlayerWon)
        {
            if ( GameManager.Instance.NumOfPlayersDeadUntilNow >= numberOfTimesPlayerCanBackToLife || GameManager.Instance.NumOfPlayersDead >= 1)
            {
                if (GameManager.Instance.NumOfPlayersDead < 1)
                {
                    Sprite newSprite = playerDeadSprite3;
                    GameObject playerDead = Utils.Instance.FindInactiveObjectByName("Player Dead");
                    SpriteRenderer spriteRenderer = playerDead.GetComponent<SpriteRenderer>();
                    spriteRenderer.sprite = newSprite;
                    playerDead.SetActive(true);
                    //turn off playerDead box collider 2D
                    playerDead.GetComponent<BoxCollider2D>().enabled = false;
                }
                AudioController.Instance.StopPlayInkInLoop();
                StartCoroutine(Die());
                Destroy(oxygenBar); 

            }
            else
            {
                TurnToDeadPlayer();
            }
        }
        if (currentOxygenValue > 0&& !isTouchingWaterEnding)
        {
            currentOxygenValue -= 1f;
        }

        oxygenBarController.updateBar(currentOxygenValue,maxTimeWithoutOxygen);

    }


    private IEnumerator Die()
    {
        Debug.Log("Player is dead");
        isDead = true;
        _animator.SetBool("isDead", true);
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    public void SetOxygenBar(GameObject newOxygenBar, float value)
    {
        oxygenBar = newOxygenBar;
        currentOxygenValue = value;
        oxygenBarController = oxygenBar.GetComponent<BarController>();
        oxygenBarController.updateBar(currentOxygenValue,maxTimeWithoutOxygen);
    }

    private void ReturnOtherPlayerToLife()
    {
        AudioController.Instance.PlayIncreaseOxygen();
        GameManager.Instance.NumOfPlayersDead--;
        currentOxygenValue /= 2;
        GameObject playerDead = Utils.Instance.FindInactiveObjectByName("Player Dead");
        playerDead.SetActive(false);
        otherPlayer=Instantiate(otherPlayerPrefab, transform.position, Quaternion.identity, null);
        otherPlayer.layer = LayerMask.NameToLayer("Player"+(3-playerType));
        otherPlayer.GetComponent<PlayerController>().SetOtherPlayer(gameObject);
        otherPlayer.GetComponent<PlayerController>().SetOtherPlayerOxygenBar(oxygenBar);
        GameObject.Find("Main Camera").GetComponent<MultipleTargetCamera>().UpdateTargets(otherPlayer.transform);
        otherPlayerOxygenBar.SetActive(true);
        otherPlayer.GetComponent<PlayerController>().SetOxygenBar(otherPlayerOxygenBar, maxTimeWithoutOxygen/2);
    }
    
    public void SetOtherPlayer(GameObject player)
    {
        otherPlayer = player;
    }
    
    public void SetOtherPlayerOxygenBar(GameObject oxygenBar)
    {
        otherPlayerOxygenBar = oxygenBar;
    }
}