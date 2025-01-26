using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MiniEnemyController : MonoBehaviour
{
    [SerializeField] private GameObject miniEnemyLifePrefab; // Prefab for the MiniEnemyLife
    private GameObject[] lives; // To keep track of instantiated life objects
    private bool isDead = false;
    private Animator _animator;
    [SerializeField] private int lifeCount; // Number of lives
    [SerializeField] private int miniEnemyType;

    void Start()
    {
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
        if (collision.gameObject.CompareTag("Shot"))
        {
            Destroy(collision.gameObject);
            DestroyLife();
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            if (miniEnemyType == 1)
            {
                EventManager.Instance.TriggerEvent(EventManager.EVENT_DECREASE_PLAYER_LIFE, gameObject);

                if (!isDead)
                {
                    AudioController.Instance.PlayExplosion();
                    // call even of dicrease life for player
                    isDead = true;
                    _animator.SetBool("isDead", isDead);
                    StartCoroutine(WaitForDeathAnimation());
                }
            }
        }
    }

    private void DestroyLife()
    {
        if (lifeCount > 0)
        {
            lifeCount--;
            if (lives[lifeCount] != null)
            {
                Destroy(lives[lifeCount]);
                lives[lifeCount] = null;
            }

            if (miniEnemyType != 1)
            {
                AudioController.Instance.PlayEnemyHit();
            }
        }

        if (lifeCount <= 0)
        {
            if (miniEnemyType == 1)
            {
                AudioController.Instance.PlayExplosion();
                isDead = true;
                _animator.SetBool("isDead", isDead);    
            }
            StartCoroutine(WaitForDeathAnimation());

        }
    }
    
    private IEnumerator WaitForDeathAnimation()
    {
        Destroy(Utils.Instance.FindUnderParentInactiveObjectByName("Ink", gameObject));
        // Wait for the length of the death animation clip
        if (miniEnemyType == 1)
        {
            yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length+0.4f);
            
        }
        
        // Call the Die method after the animation finishes
        Die();
    }
    

    private void Die()
    {
        Debug.Log("Mini Enemy is dead");
        Destroy(gameObject);
    }
}
