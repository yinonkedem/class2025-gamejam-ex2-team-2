using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MiniEnemyController : MonoBehaviour
{
    [SerializeField] private GameObject miniEnemyLifePrefab; // Prefab for the MiniEnemyLife
    [SerializeField] private int lifeCount = 2; // Number of lives

    private GameObject[] lives; // To keep track of instantiated life objects
    private bool isDead = false;
    private Animator _animator;
    
    void Start()
    {
        // Initialize lives
        lives = new GameObject[lifeCount];
        for (int i = 0; i < lifeCount; i++)
        {
            // Instantiate life prefabs as children of the specified parent or this object
            Vector3 offset = new Vector3(i * 0.5f, -1, 0); // Slight offset for visualization
            lives[i] = Instantiate(miniEnemyLifePrefab, transform.position + offset, Quaternion.identity, transform);
        }
        _animator = GetComponent<Animator>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // if the othertag is with tag shot socall to decrease life function
        if (collision.gameObject.CompareTag("Shot"))
        {
            //desrtroy the shot
            Destroy(collision.gameObject);
            DestroyLife();
        }

        if (collision.gameObject.CompareTag("Player"))
        {                
            EventManager.Instance.TriggerEvent(EventManager.EVENT_DECREASE_PLAYER_LIFE, gameObject);

            if (!isDead)
            {
                // call even of dicrease life for player
                isDead = true;
                _animator.SetBool("isDead", isDead);
                StartCoroutine(WaitForDeathAnimation());
            }
        }
    }

    private void DestroyLife()
    {
        // Reduce life count and destroy the last life prefab if it exists
        if (lifeCount > 0)
        {
            lifeCount--;
            if (lives[lifeCount] != null)
            {
                Destroy(lives[lifeCount]);
                lives[lifeCount] = null;
            }
        }

        if (lifeCount <= 0)
        {
            isDead = true;
            _animator.SetBool("isDead", isDead);
            StartCoroutine(WaitForDeathAnimation());

        }
    }
    
    private IEnumerator WaitForDeathAnimation()
    {
        Destroy(Utils.Instance.FindUnderParentInactiveObjectByName("Ink", gameObject));
        // Wait for the length of the death animation clip
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length+0.4f);
        
        // Call the Die method after the animation finishes
        Die();
    }
    

    private void Die()
    {
        Debug.Log("Mini Enemy is dead");
        Destroy(gameObject);
    }
}
