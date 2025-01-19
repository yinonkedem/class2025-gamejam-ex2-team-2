using Unity.VisualScripting;
using UnityEngine;

public class MiniEnemyController : MonoBehaviour
{
    [SerializeField] private GameObject miniEnemyLifePrefab; // Prefab for the MiniEnemyLife
    [SerializeField] private int lifeCount = 2; // Number of lives

    private GameObject[] lives; // To keep track of instantiated life objects

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

        // If no lives remain, destroy the MiniEnemy
        if (lifeCount <= 0)
        {
            Destroy(gameObject);
        }
    }
}