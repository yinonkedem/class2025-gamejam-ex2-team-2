using System.Collections.Generic;
using UnityEngine;

public class WaterZone : MonoBehaviour
{
    [SerializeField] private float waterSpeedMultiplier = 0.5f; // Speed multiplier for swimming
    [SerializeField] private float swimmingVerticalSpeed = 3f;  // Vertical speed for swimming

    private HashSet<PlayerMovment1> playersInWater = new HashSet<PlayerMovment1>(); // Track players currently in water

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovment1 playerMovement = other.GetComponent<PlayerMovment1>();
        if (playerMovement != null)
        {
            // Player enters the water
            if (!playersInWater.Contains(playerMovement))
            {
                playersInWater.Add(playerMovement);
                playerMovement.OnEnterWater(swimmingVerticalSpeed, waterSpeedMultiplier);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerMovment1 playerMovement = other.GetComponent<PlayerMovment1>();
        if (playerMovement != null)
        {
            // Player exits the water
            if (playersInWater.Contains(playerMovement))
            {
                playersInWater.Remove(playerMovement);
                playerMovement.OnExitWater();
            }
        }
    }
}