using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

//This class should switch attack each x seconds and do T seconds without attacks between each cswitch

public class AttacksController : MonoBehaviour
{
    
    [SerializeField] private float switchInterval = 20f; // Time in seconds between each attack switch
    [SerializeField] private float pauseDuration = 10f;  // Time in seconds to pause between switches
    [SerializeField] private int numberOfAttacks = 3;  // Time in seconds to pause between switches
    [SerializeField] private int maxAttackLevel = 3;  // Time in seconds to pause between switches


    private bool isPaused = false;
    private int currentAttack = -1;
    private bool isSeAttackApplied = false;
    private Animator _animator;
    private int attackLevel =1;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        StartCoroutine(SwitchAttacks());
    }

    void Update()
    {
        _animator.SetInteger("numberOfAttack", currentAttack);
        _animator.SetInteger("attackLevel", attackLevel);
    }
    

    private IEnumerator SwitchAttacks()
    {
        while (true)
        {
            for (int i = 1; i <= numberOfAttacks; i++)
            {
                
                // Switch attack logic here
                Debug.Log("Switching to: "+i+" attack");
                    
                currentAttack = i;
                if (i > maxAttackLevel)
                {
                    attackLevel = maxAttackLevel;
                }
                else
                { 
                    attackLevel = i;
                }
                
                yield return new WaitForSeconds(switchInterval);
                currentAttack = -1;
                yield return new WaitForSeconds(pauseDuration);
            }

        }
    }
}

