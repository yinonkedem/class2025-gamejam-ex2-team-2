using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAwarenessController : MonoBehaviour
{
    public bool AwareOfPlayer { get; private set; }
    public Vector2 DirectionToPlayer { get; private set; }

    [SerializeField]
    private float _playerAwarenessDistance;

    private Transform[] _players;


    private void Awake()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        _players = new Transform[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            _players[i] = players[i].transform;
        }
    }
    

    // // Update is called once per frame
    // void Update()
    // {
    //     Transform closestPlayer = null;
    //     float closestDistance = _playerAwarenessDistance;
    //
    //     foreach (Transform player in _players)
    //     {
    //         if(player!=null)
    //         {
    //             Vector2 enemyToPlayerVector = player.position - transform.position;
    //             float distanceToPlayer = enemyToPlayerVector.magnitude;
    //
    //             if (distanceToPlayer <= closestDistance)
    //             {
    //                 Debug.Log("enemy is close");
    //                 GameObject ink = Utils.Instance.FindUnderParentInactiveObjectByName("Ink", gameObject);
    //                 ink.SetActive(true);
    //                 closestDistance = distanceToPlayer;
    //                 closestPlayer = player;
    //                 DirectionToPlayer = enemyToPlayerVector.normalized;
    //             }
    //         }
    //
    //     }
    //
    //     AwareOfPlayer = closestPlayer != null;
    // }
    
    
    void Update()
    {
        Transform closestPlayer = null;
        float closestDistance = _playerAwarenessDistance;
        bool isPlayerClose = false;

        foreach (Transform player in _players)
        {
            if (player != null)
            {
                Vector2 enemyToPlayerVector = player.position - transform.position;
                float distanceToPlayer = enemyToPlayerVector.magnitude;

                if (distanceToPlayer <= closestDistance)
                {
                    Debug.Log("Enemy is close");
                    GameObject ink = Utils.Instance.FindUnderParentInactiveObjectByName("Ink", gameObject);
                    if (ink != null)
                    {
                        ink.SetActive(true);
                    }
                    closestDistance = distanceToPlayer;
                    closestPlayer = player;
                    DirectionToPlayer = enemyToPlayerVector.normalized;
                    isPlayerClose = true;
                }
            }
        }

        // If no players are close, deactivate the ink
        if (!isPlayerClose)
        {
            GameObject ink = Utils.Instance.FindUnderParentInactiveObjectByName("Ink", gameObject);
            if (ink != null)
            {
                ink.SetActive(false);
            }
        }

        AwareOfPlayer = closestPlayer != null;
    }

    
    
}







