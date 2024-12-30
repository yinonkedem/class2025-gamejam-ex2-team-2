using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private float timeToAllowPlayerToAttack = 60f;
    [SerializeField] private float timeOfAttack = 10f;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("CallAllowPinkPlayerToAttack", timeToAllowPlayerToAttack, timeToAllowPlayerToAttack);
        InvokeRepeating("CallAllowGreyPlayerToAttack", timeToAllowPlayerToAttack, timeToAllowPlayerToAttack);
    }

    // Update is called once per frame
    void Update()
    {
       
        
    }

    private void CallAllowPinkPlayerToAttack()
    {
        EventManager.Instance.TriggerEvent(EventManager.EVENT_ADD_ATTACK_TO_PINK_PLAYER, gameObject);
    }

    private void CallAllowGreyPlayerToAttack()
    {
        EventManager.Instance.TriggerEvent(EventManager.EVENT_ADD_ATTACK_TO_GREY_PLAYER, gameObject);
    }
    public float GetTimeOfAttack()
    {
        return timeOfAttack;
    }
}
