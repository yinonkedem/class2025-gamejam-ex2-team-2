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
    [SerializeField] private float timeEnemyPrepareToAttack = 2f;  // Time in seconds to pause between switches
    [SerializeField] private float timeBetweenTargetToBolt = 2f;  // Time in seconds to pause between switches
    [SerializeField] private GameObject attackTargetPrefab;
    [SerializeField] private GameObject boltAttackPrefab;

    
    private Dictionary<int, System.Action> attacks = new Dictionary<int, System.Action>();
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
        attacks.Add(1, StartBoltAttack);
    }


    // After timeEnemyPrepareToAttack seconds, create instance of the prefab : attackTarget and place it one one of the players location\
    // then after one second instantiate the attack prefab : boltAttack where the target placed 
    private void StartBoltAttack()
    {
        StartCoroutine(PrepareAndExecuteBoltAttack());
    }

    private IEnumerator PrepareAndExecuteBoltAttack()
    {
        // Wait for the preparation time
        yield return new WaitForSeconds(timeEnemyPrepareToAttack);

        // Create an instance of the attack target at one of the player's locations
        GameObject attackTarget = Instantiate(attackTargetPrefab, GetRandomPlayerPosition(), Quaternion.identity, GameObject.Find("Main").transform);

        // Wait for one second
        yield return new WaitForSeconds(timeBetweenTargetToBolt);

        // Instantiate the bolt attack at the target's position
        GameObject boltAttack = Instantiate(boltAttackPrefab, attackTarget.transform.position, Quaternion.identity, GameObject.Find("Main").transform);

        // Optionally, destroy the attack target after the bolt attack is instantiated
        Destroy(attackTarget);

        float boltAnimationTime =  boltAttack.GetComponent<Animator>().runtimeAnimatorController.animationClips[0].length;
        
        
        yield return new WaitForSeconds(boltAnimationTime);

        Destroy(boltAttack);
        currentAttack = -1;
        _animator.SetInteger("numberOfAttack", currentAttack);

    }

    private Vector3 GetRandomPlayerPosition()
    {
        // Assuming you have a way to get all player objects in the scene
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        // Select a random player
        GameObject randomPlayer = players[Random.Range(0, players.Length)];
        return randomPlayer.transform.position;
    }


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
                attacks[i].Invoke();
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
