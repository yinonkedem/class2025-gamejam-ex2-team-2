using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;


public class AttacksController : MonoBehaviour
{


    [SerializeField] private int numberOfBoltsInSingleAttack = 2;
    [SerializeField] private float timeBetweenEachBoltAttackRound = 10f;
    [SerializeField] private float timeEnemyPrepareToAttack = 3f;
    [SerializeField] private float timeBetweenTargetToBolt = 2f; 
    [SerializeField] private GameObject attackTargetPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject enemyPrefabType2;
    [SerializeField] private GameObject boltAttackPrefab;
    [SerializeField] private float increaseEnemySpeed = 0.5f;
    [SerializeField] private float increaseEnemySize = 0.4f;
    [SerializeField] private float timeBetweenEachCreationOfMiniEnemies = 15f;
    
    private Animator _animator;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        Invoke("StartCoroutineDelayed", 5f);
    }
    
    

    public void StartMiniEnemiesAttack()
    {
        Debug.Log("Start Mini Enemies Attack");
        StartCoroutine(PrepareAndExecuteExtraMiniEnemiesAttack());
    }

    
    private IEnumerator PrepareAndExecuteExtraMiniEnemiesAttack()
    {

        while (true)
        {
            _animator.SetBool("isPrepareToAttack", true);
            yield return new WaitForSeconds(timeEnemyPrepareToAttack);

            GameObject miniEnemy=Instantiate(enemyPrefab, transform.position, Quaternion.identity, GameObject.Find("Enemies").transform);
            GameObject.Find("Main Camera").GetComponent<MultipleTargetCamera>().UpdateTargets(miniEnemy.transform);
            
            GameObject miniEnemyType2=Instantiate(enemyPrefabType2, transform.position, Quaternion.identity, GameObject.Find("Enemies").transform);
            GameObject.Find("Main Camera").GetComponent<MultipleTargetCamera>().UpdateTargets(miniEnemyType2.transform);
            
            _animator.SetBool("isPrepareToAttack", false);
            yield return new WaitForSeconds(timeBetweenEachCreationOfMiniEnemies);
        }

    }

    
    public void StartBoltAttack()
    {
        Debug.Log("Start Bolt Attack");
        StartCoroutine(PrepareAndExecuteBoltAttack());
    }


    
    private IEnumerator PrepareAndExecuteBoltAttack()
    {
        transform.localScale += new Vector3(increaseEnemySize, increaseEnemySize, increaseEnemySize);
        GetComponent<EnemyMovement>().AddToSpeed(increaseEnemySpeed);


        
        // Get references to both players
        GameObject[] players;

        while (true)
        {
            _animator.SetBool("isPrepareToAttack", true);
            yield return new WaitForSeconds(timeEnemyPrepareToAttack);
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
                    float randomX = Mathf.Clamp(Random.Range(GameManager.Instance.LeftWallPosition, GameManager.Instance.RightWallPosition),
                        GameManager.Instance.LeftWallPosition, GameManager.Instance.RightWallPosition);
                    float randomY = Mathf.Clamp(Random.Range(GameManager.Instance.GroundPosition, GameManager.Instance.WaterEndingPosition),
                        GameManager.Instance.GroundPosition, GameManager.Instance.WaterEndingPosition);

                    Vector3 randomPosition = new Vector3(randomX, randomY, playerPosition.z); // Maintain Z position
                    GameObject additionalTarget = Instantiate(attackTargetPrefab, randomPosition, Quaternion.identity, GameObject.Find("Main").transform);
                    attackTargets.Add(additionalTarget);
                }
            }

            yield return new WaitForSeconds(timeBetweenTargetToBolt);

            _animator.SetBool("isPrepareToAttack", false);

            // Create bolts for all targets simultaneously
            List<GameObject> bolts = new List<GameObject>();
            foreach (GameObject target in attackTargets)
            {
                AudioController.Instance.PlayBoltAttack();
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
        
    }

}
