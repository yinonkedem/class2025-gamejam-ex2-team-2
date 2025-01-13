using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;


public class AttacksController : MonoBehaviour
{


    [SerializeField] private int numberOfBoltsInSingleAttack = 2;
    [SerializeField] private float timeBetweenEachBoltInTheAttack = 5f;
    [SerializeField] private float pauseDuration = 20f; 
    [SerializeField] private int numberOfAttacks = 1;  
    [SerializeField] private int numberOfMiniEnemies = 2;
    [SerializeField] private float timeToStayMiniEnemies = 40f;  
    [SerializeField] private int maxAttackLevel = 3;  
    [SerializeField] private float timeEnemyPrepareToAttack = 2f;
    [SerializeField] private float timeBetweenTargetToBolt = 2f; 
    [SerializeField] private float timeUntilFirdtAttackStart = 5f;
    [SerializeField] private GameObject attackTargetPrefab;
    [SerializeField] private GameObject enemyPrefab;
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
        attacks.Add(2, StartExtraMiniEnemiesAttack);
    }
    
    private void StartExtraMiniEnemiesAttack()
    {
        //check if there exists more than 1 gameobjects with tag Mini Enemy
        if (GameObject.FindGameObjectsWithTag("Mini Enemy").Length >= numberOfMiniEnemies)
        {
            return;
        }
        StartCoroutine(PrepareAndExecuteExtraMiniEnemiesAttack());
    }

    private IEnumerator PrepareAndExecuteExtraMiniEnemiesAttack()
    {
        //create list of gameobject of sixe numberOfMiniEnemies
        GameObject[] miniEnemies = new GameObject[numberOfMiniEnemies];
        for (int i = 0; i < numberOfMiniEnemies; i++)
        {
            GameObject miniEnemy=Instantiate(enemyPrefab, transform.position, Quaternion.identity, GameObject.Find("Enemies").transform);
            miniEnemies[i] = miniEnemy;
            GameObject.Find("Main Camera").GetComponent<MultipleTargetCamera>().UpdateTargets(miniEnemy.transform);
        }

        
        yield return new WaitForSeconds(timeToStayMiniEnemies) ;
        foreach (GameObject miniEnemy in miniEnemies)
        {
            Destroy(miniEnemy);
        }
    }

    
    private void StartBoltAttack()
    {
        StartCoroutine(PrepareAndExecuteBoltAttack());
    }

    private IEnumerator PrepareAndExecuteBoltAttack()
    {
        yield return new WaitForSeconds(timeEnemyPrepareToAttack);
        for (int i = 0; i < numberOfBoltsInSingleAttack; i++)
        {
            GameObject attackTarget = Instantiate(attackTargetPrefab, GetRandomPlayerPosition(), Quaternion.identity, GameObject.Find("Main").transform);
            yield return new WaitForSeconds(timeBetweenTargetToBolt);
            GameObject boltAttack = Instantiate(boltAttackPrefab, attackTarget.transform.position, Quaternion.identity, GameObject.Find("Main").transform);
            Destroy(attackTarget);
            float boltAnimationTime =  boltAttack.GetComponent<Animator>().runtimeAnimatorController.animationClips[0].length;
            yield return new WaitForSeconds(boltAnimationTime);
            Destroy(boltAttack);
            yield return new WaitForSeconds(timeBetweenEachBoltInTheAttack);
        }
        currentAttack = -1; 
        _animator.SetInteger("numberOfAttack", currentAttack);
    }

    private Vector3 GetRandomPlayerPosition()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
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
        yield return new WaitForSeconds(timeUntilFirdtAttackStart);  // Initial pause
    
        while (true)
        {
            for (int i = 1; i <= numberOfAttacks; i++)
            {
                currentAttack = i;
                attacks[i].Invoke();
                _animator.SetInteger("numberOfAttack", currentAttack);
                Debug.Log($"Switching to Attack {currentAttack}");
            }
    
            currentAttack = -1;
            _animator.SetInteger("numberOfAttack", currentAttack);
            Debug.Log("Attack paused.");
    
            yield return new WaitForSeconds(pauseDuration);
        }
    }
}
