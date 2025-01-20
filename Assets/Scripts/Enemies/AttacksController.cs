using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;


public class AttacksController : MonoBehaviour
{


    [SerializeField] private int numberOfBoltsInSingleAttack = 2;
    [SerializeField] private float timeBetweenEachBoltAttackRound = 10f;
    [SerializeField] private int numberOfMiniEnemies = 2;
    [SerializeField] private float timeToStayMiniEnemies = 40f;  
    [SerializeField] private float timeEnemyPrepareToAttack = 2f;
    [SerializeField] private float timeBetweenTargetToBolt = 2f; 
    [SerializeField] private float timeUntilFirdtAttackStart = 5f;
    [SerializeField] private GameObject attackTargetPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject boltAttackPrefab;
    [SerializeField] private int numberOfBoltsAttackRounded = 4;

    
    private Dictionary<int, System.Action> attacks = new Dictionary<int, System.Action>();
    private int currentAttack = -1;
    private Animator _animator;

    private bool isAttackOver = false;
    //private int attackLevel =1;
    
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        Invoke("StartCoroutineDelayed", 5f);
        attacks.Add(-1, StartSwimmingWithoutAttacks);
        attacks.Add(2, StartExtraMiniEnemiesAttack);
        attacks.Add(1, StartBoltAttack);
    }
    
    private void StartSwimmingWithoutAttacks()
    {
        //wait for 20 seconds
        StartCoroutine(WaitForSwimmingWithoutAttacks());
    }
    
    private IEnumerator WaitForSwimmingWithoutAttacks()
    {
        yield return new WaitForSeconds(10f);
        isAttackOver = true;

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
        isAttackOver = true;
    }

    
    private void StartBoltAttack()
    {
        StartCoroutine(PrepareAndExecuteBoltAttack());
    }


    
private IEnumerator PrepareAndExecuteBoltAttack()
{
    yield return new WaitForSeconds(timeEnemyPrepareToAttack);
    
    //get game object with tag Right  Wwall position
    
    float rightWallPosition = GameObject.FindGameObjectWithTag("Right Wall").transform.position.x;
    float leftWallPosition = GameObject.FindGameObjectWithTag("Left Wall").transform.position.x;
    float groundPosition = GameObject.FindGameObjectWithTag("Bottom Wall").transform.position.y;
    float waterEndingPosition = GameObject.FindGameObjectWithTag("Water Ending").transform.position.y;



    // Get references to both players
    GameObject[] players;

    for (int i = 0; i < numberOfBoltsAttackRounded; i++)
    {
        List<GameObject> attackTargets = new List<GameObject>();
        players = GameObject.FindGameObjectsWithTag("Player");
        // Create 1 bolt on each player's position
        foreach (GameObject player in players)
        {
            Vector3 playerPosition = player.transform.position;
            GameObject playerTarget = Instantiate(attackTargetPrefab, playerPosition, Quaternion.identity, GameObject.Find("Main").transform);
            attackTargets.Add(playerTarget);

            // Create 3 additional random targets near each player within the specified range
            for (int j = 0; j < (numberOfBoltsInSingleAttack-1)/2; j++)
            {
                // Ensure random position is within the range
                float randomX = Mathf.Clamp(Random.Range(leftWallPosition, rightWallPosition), leftWallPosition, rightWallPosition);
                float randomY = Mathf.Clamp(Random.Range(groundPosition, waterEndingPosition), groundPosition, waterEndingPosition);

                Vector3 randomPosition = new Vector3(randomX, randomY, playerPosition.z); // Maintain Z position
                GameObject additionalTarget = Instantiate(attackTargetPrefab, randomPosition, Quaternion.identity, GameObject.Find("Main").transform);
                attackTargets.Add(additionalTarget);
            }
        }

        yield return new WaitForSeconds(timeBetweenTargetToBolt);

        // Create bolts for all targets simultaneously
        List<GameObject> bolts = new List<GameObject>();
        foreach (GameObject target in attackTargets)
        {
            GameObject boltAttack = Instantiate(boltAttackPrefab, target.transform.position, Quaternion.identity, GameObject.Find("Main").transform);
            bolts.Add(boltAttack);
            Destroy(target); // Destroy the target after creating the bolt
        }

        // Wait for bolt animations to complete
        float boltAnimationTime = bolts[0].GetComponent<Animator>().runtimeAnimatorController.animationClips[0].length;
        yield return new WaitForSeconds(boltAnimationTime);

        // Destroy all bolts
        foreach (GameObject bolt in bolts)
        {
            Destroy(bolt);
        }

        yield return new WaitForSeconds(timeBetweenEachBoltAttackRound);
    }
    
    currentAttack = -1;
    _animator.SetInteger("numberOfAttack", currentAttack);
    isAttackOver = true;
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
            
            foreach (int attackIndex in attacks.Keys)
            {
                currentAttack = attackIndex;
                attacks[attackIndex].Invoke();
                _animator.SetInteger("numberOfAttack", currentAttack);
                Debug.Log($"Switching to Attack {currentAttack}");
                //not continue until isAttackOver is true
                while (!isAttackOver)
                {
                    yield return null;
                }
                isAttackOver = false;
            }
        }
    }
}
