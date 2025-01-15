using System.Collections;
using UnityEngine;

public class Boomerang : MonoBehaviour
{
    public Transform shooter;
    public float speed = 10f;
    public float maxDistance = 10f;
    public float hangTime = 1f; // Time to hang in the air
    private Rigidbody rb;
    private Vector3 startPosition;
    private bool isReturning = false;
    private bool isHanging = false;
    private float distanceTraveled = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        MoveTowardsTarget();
    }

    void Update()
    {
        if (!isReturning && !isHanging)
        {
            distanceTraveled += speed * Time.deltaTime;
            if (distanceTraveled >= maxDistance)
            {
                StartCoroutine(HangInAir());
            }
        }
    }

    void MoveTowardsTarget()
    {
        rb.velocity = transform.forward * speed; // Ensure the boomerang moves in the forward direction
    }

    IEnumerator HangInAir()
    {
        isHanging = true;
        rb.velocity = Vector3.zero; // Stop moving
        yield return new WaitForSeconds(hangTime);
        isHanging = false;
        ReturnToShooter();
    }

    void ReturnToShooter()
    {
        isReturning = true;
        Vector3 returnDirection = (shooter.position - transform.position).normalized;
        rb.velocity = returnDirection * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Trigger hit effect, then return
            TriggerHitEffect();
            ReturnToShooter();
        }
    }

    private void TriggerHitEffect()
    {
        // Placeholder for hit effect logic
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform == shooter && isReturning)
        {
            Destroy(gameObject); // Destroy the boomerang when it returns to the player
        }
    }
}