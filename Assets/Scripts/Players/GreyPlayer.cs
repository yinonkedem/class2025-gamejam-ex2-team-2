using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreyPlayer : MonoBehaviour
{
    [SerializeField] private GameObject greyAttackPrefab;
    [SerializeField] private KeyCode attackKey;
    [SerializeField] private Color colorOfTheAttack = Color.green;


    private GameObject currentAttack; 
    private bool isUnderAttack = false;
    private bool isAttacking = false;
    private bool isHaveAnAttack = false;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.StartListening(EventManager.EVENT_GREY_PLAYER_HIT_FROM_ATTACK, HitFromAttack);
        EventManager.Instance.StartListening(EventManager.EVENT_GREY_PLAYER_DIE, Die);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(attackKey) && isHaveAnAttack)
        {
            Attack(gameObject);
        }
    }

    private void Attack(GameObject obj)
    {

    }
    

    private void HitFromAttack(GameObject obj)
    {
        //TODO : start animation
        isUnderAttack = true;
    }
    

    private void Die(GameObject arg0)
    {
        Debug.Log("Grey player is dead");
    }


}
