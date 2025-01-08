using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private int maxLife = 3;
    private int life = 0;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // if the othertag is with tag shot socall to decrease life function
        if (collision.gameObject.CompareTag("Shot"))
        {
            //desrtroy the shot
            Destroy(collision.gameObject);
            DecreaseLife();
        }
    }

    private void DecreaseLife()
    {
        Debug.Log("Enemy life decreased");
    }
}
