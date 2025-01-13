using UnityEngine;

namespace Players.Shoot
{
    public class Shooter : MonoBehaviour
    {
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform target;
        [SerializeField] private float shootRate;
        [SerializeField] private float projectileMoveSpeed;
        private float shootTimer;

        private void Update()
        {
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0)
            {
                shootTimer = shootRate;
                Projectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity).GetComponent<Projectile>();
                projectile.InitializeProjectile(target, projectileMoveSpeed);
            }
        }

    }
}
