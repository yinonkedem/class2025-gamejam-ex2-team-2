using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterZone : MonoBehaviour
{
    [SerializeField] private float waterSpeedMultiplier = 0.5f; // Speed multiplier for swimming
    [SerializeField] private float swimmingVerticalSpeed = 3f;  // Vertical speed for swimming

    private List<PlayerMovment1> players = new List<PlayerMovment1>(); // List of players in the game

    private void Start()
    {
        // Find all players in the game and add them to the list
        PlayerMovment1[] foundPlayers = FindObjectsOfType<PlayerMovment1>();
        players.AddRange(foundPlayers);

        if (players.Count == 0)
        {
            Debug.LogWarning("No players found in the scene.");
        }
    }

    private void Update()
    {
        foreach (PlayerMovment1 playerMovement in players)
        {
            if (playerMovement == null) continue;

            Transform playerTransform = playerMovement.transform;

            // Check if the player is below the water zone
            if (playerTransform.position.y < transform.position.y)
            {
                playerMovement.OnEnterWater(swimmingVerticalSpeed, waterSpeedMultiplier);
            }
            else
            {
                playerMovement.OnExitWater();
            }
        }
    }
}