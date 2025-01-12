using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private float timeToAllowPlayerToAttack;
    [SerializeField] private float timeOfAttack = 10f;
    
    private bool arePlayerWon = false;

    private bool arePlayersDefeated = false;


    //create getter and setters for the private variables
    
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
        Debug.Log("Players Length: " + players.Length);
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
 
    public void HitEnemiesInTheWater()
    {
        //TODO : add enmy animation 

        Debug.Log("Hit Enemies In The Water");
        GameObject enemies = GameObject.Find("Enemies");
        foreach (Transform enemy in enemies.transform)
        {
            StartCoroutine(enemy.GetComponent<EnemyMovement>().StopMovement(timeOfAttack));
        }
    }
}
