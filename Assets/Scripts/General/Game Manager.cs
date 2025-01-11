using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private float timeToAllowPlayerToAttack;
    [SerializeField] private float timeOfAttack = 10f;
    
    private bool arePlayerWon = false;
    private bool arePlayerOneDefeated = false;
    private bool arePlayerTwoDefeated = false;
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
    
    public void CheckGameOver()
    {
        if (arePlayerOneDefeated && arePlayerTwoDefeated)
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
