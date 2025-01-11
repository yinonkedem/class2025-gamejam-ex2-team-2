using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private int maxLife = 3;
    [SerializeField] private int decreaseLifeCountWhenGetHit = 1;
    [SerializeField] private GameObject lifeBar;

    private int currentLife;
    private BarController lifeBarController;

    private void Start()
    {
        currentLife = maxLife;
        lifeBarController = lifeBar.GetComponent<BarController>();
        lifeBarController.updateBar(currentLife,maxLife);

    }

    private void Update()
    {
        UpdateLifeBarPosition();

    }
    
    private void UpdateLifeBarPosition()
    {
        // Set the oxygen bar's position relative to the player
        Vector3 lifeBarPosition = transform.position; // Get the player's position
        lifeBarPosition.y += 2f;  // Offset to place it above the player
        lifeBar.transform.position = lifeBarPosition;  // Update oxygen bar's position
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // if the othertag is with tag shot socall to decrease life function
        if (collision.gameObject.CompareTag("Shot"))
        {
            //desrtroy the shot
            Destroy(collision.gameObject);
            DecreaseLife();
        }
    }

    private void DecreaseLife()
    {
        currentLife -= decreaseLifeCountWhenGetHit;
        lifeBarController.updateBar(currentLife,maxLife);
        Debug.Log("Enemy life decreased");
    }
}
