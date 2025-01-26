using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Player Movement")]
public class PlayerMovementStats : ScriptableObject
{

    [Header("Dash")]
    [Range(0f, 1f)] public float DashTime = 0.11f;
    [Range(1f, 200f)] public float DashSpeed = 40f;
    
    [Header("Swimming Settings")]
    public float swimVerticalSpeed = 3f;
    public float waterGravityScale = 0.5f;
    
    [Header("Swimming Physics")]
    public float swimMaxSpeed = 5f;       // Maximum horizontal swimming speed
    public float swimUpwardSpeed = 2f;   // Speed for upward movement
    public float swimDownwardSpeed = 4f; // Speed for downward movement
    public float gravityFeeling = 1.5f;  // Downward force when no input is provided
    public float maxSinkSpeed = 3f;      // Maximum sinking speed due to gravity
    public float swimAcceleration = 10f; // Acceleration for smooth movement
    
    
    [Header("Gravity")]
    public float defaultGravityScale = 3f; // Added this line for swimming
    
    public readonly Vector2[] DashDirections = new Vector2[]
    {
        new Vector2(0, 0), // Nothing
        new Vector2(1, 0), // Right
        new Vector2(1, 1).normalized, // Top-Right
        new Vector2(0, 1), // Up
        new Vector2(-1, 1).normalized, // Top-Left
        new Vector2(-1, 0), // Left
        new Vector2(-1, -1).normalized, // Bottom-Left
        new Vector2(0, -1), // Down
        new Vector2(1, -1).normalized // Bottom-Right
    };
    
}