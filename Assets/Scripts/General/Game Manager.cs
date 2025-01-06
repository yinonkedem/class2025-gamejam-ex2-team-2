using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private float timeToAllowPlayerToAttack;
    [SerializeField] private float timeOfAttack = 10f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
       
        
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
