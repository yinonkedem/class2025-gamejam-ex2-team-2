using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private float timeToAllowPlayerToAttack;
    [SerializeField] private float timeOfAttack = 10f;
    
    private bool arePlayerWon = false;
    private bool arePlayersDefeated = false;
    private float rightWallPosition;
    private float leftWallPosition;
    private float groundPosition;
    private float waterEndingPosition;
    
    //getters for the private variables
    public float RightWallPosition => rightWallPosition;
    public float LeftWallPosition => leftWallPosition;
    public float GroundPosition => groundPosition;
    public float WaterEndingPosition => waterEndingPosition;
    public float TimeToAllowPlayerToAttack => timeToAllowPlayerToAttack;

    //create getter and setters for the private variables

    private void Awake()
    {
        //get the position of the walls and the ground
        rightWallPosition = GameObject.FindGameObjectWithTag("Right Wall").transform.position.x;
        leftWallPosition = GameObject.FindGameObjectWithTag("Left Wall").transform.position.x;
        groundPosition = GameObject.FindGameObjectWithTag("Bottom Wall").transform.position.y;
        waterEndingPosition = GameObject.FindGameObjectWithTag("Water Ending").transform.position.y;
    }

    public bool ArePlayerWon
    {
        get => arePlayerWon;
        set => arePlayerWon = value;
    }
    
    public bool ArePlayersDefeated
    {
        get => arePlayersDefeated;
        set => arePlayersDefeated = value;
    }
    
    private void Update()
    {
        CheckGameOver();
    }
    
    public void CheckGameOver()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        // Debug.Log("Players Length: " + players.Length);
        // if there is not object with tag Player
        if (players.Length == 0)
        {
            arePlayersDefeated = true;
            Debug.Log("Game Over");
            ScreenChanger.Instance.ActivateGameOver();
        }
    }
    
    
    public float GetTimeOfAttack()
    {
        return timeOfAttack;
    }

}
