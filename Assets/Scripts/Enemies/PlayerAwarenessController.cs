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
        PlayerMovement[] playerMovements = FindObjectsOfType<PlayerMovement>();
        _players = new Transform[playerMovements.Length];
        for (int i = 0; i < playerMovements.Length; i++)
        {
            _players[i] = playerMovements[i].transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Transform closestPlayer = null;
        float closestDistance = _playerAwarenessDistance;

        foreach (Transform player in _players)
        {
            Vector2 enemyToPlayerVector = player.position - transform.position;
            float distanceToPlayer = enemyToPlayerVector.magnitude;

            if (distanceToPlayer <= closestDistance)
            {
                closestDistance = distanceToPlayer;
                closestPlayer = player;
                DirectionToPlayer = enemyToPlayerVector.normalized;
            }
        }

        AwareOfPlayer = closestPlayer != null;
    }
}







