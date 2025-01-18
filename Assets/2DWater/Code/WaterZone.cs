using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterZone : MonoBehaviour
{
    [SerializeField] private Transform player;  // Reference to the player's Transform
    [SerializeField] private float waterSpeedMultiplier = 0.5f; // Speed multiplier for swimming
    [SerializeField] private float swimmingVerticalSpeed = 3f;  // Vertical speed for swimming

    private void Update()
    {
        if (player == null)
        {
            Debug.LogWarning("Player Transform is not assigned in WaterZone script!");
            return;
        }

        // Check if the player is below the water zone
        if (player.position.y < transform.position.y)
        {
            // Player is swimming
            PlayerMovment1 playerMovement = player.GetComponent<PlayerMovment1>();
            if (playerMovement != null)
            {
                playerMovement.OnEnterWater(swimmingVerticalSpeed, waterSpeedMultiplier);
            }
        }
        else
        {
            // Player is above the water zone
            PlayerMovment1 playerMovement = player.GetComponent<PlayerMovment1>();
            if (playerMovement != null)
            {
                playerMovement.OnExitWater();
            }
        }
    }
}
