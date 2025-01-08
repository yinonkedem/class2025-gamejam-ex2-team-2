using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;


public class AttacksController : MonoBehaviour
{
    
    
    [SerializeField] private float switchInterval = 20f; // Time in seconds between each attack switch
    [SerializeField] private float pauseDuration = 10f;  // Time in seconds to pause between switches
    [SerializeField] private int numberOfAttacks = 1;  // Time in seconds to pause between switches
    [SerializeField] private int maxAttackLevel = 3;  // Time in seconds to pause between switches
    
    
    private bool isPaused = false;
    private int currentAttack = -1;
    private bool isSeAttackApplied = false;
    private Animator _animator;
    //private int attackLevel =1;
    
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        Invoke("StartCoroutineDelayed", 5f);
    }
    
    // void Update()
    // {
    //     // Force update the Animator parameter every frame
    //     _animator.SetInteger("numberOfAttack", currentAttack);
    // }
    

    void StartCoroutineDelayed()
    {
        _animator.SetInteger("numberOfAttack", currentAttack);
        StartCoroutine(SwitchAttacks());
    }

    
    private IEnumerator SwitchAttacks()
    {
        yield return new WaitForSeconds(pauseDuration);  // Initial pause
    
        while (true)
        {
            for (int i = 1; i <= numberOfAttacks; i++)
            {
                // Set the current attack
                currentAttack = i;
                _animator.SetInteger("numberOfAttack", currentAttack);
                Debug.Log($"Switching to Attack {currentAttack}");
    
                // Wait for the switch interval
                yield return new WaitForSeconds(switchInterval);
            }
    
            // Reset to -1 for pause duration
            currentAttack = -1;
            _animator.SetInteger("numberOfAttack", currentAttack);
            Debug.Log("Attack paused.");
    
            // Wait for the pause duration
            yield return new WaitForSeconds(pauseDuration);
        }
    }
}
