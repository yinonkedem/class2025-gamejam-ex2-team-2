using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTriggerHandler : MonoBehaviour
{
    [SerializeField] private LayerMask _waterMask;
    [SerializeField] private GameObject _splashParticles;

    private EdgeCollider2D _edgeCol;
    private InteractableWater _water;

    private void Awake()
    {
        _edgeCol = GetComponent<EdgeCollider2D>();
        _water = GetComponent<InteractableWater>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If our collision gameObject is within the waterMask LayerMask
        if ((_waterMask.value & (1 << collision.gameObject.layer)) > 0)
        {
            Rigidbody2D rb = collision.GetComponentInParent<Rigidbody2D>();
            if (rb != null)
            {
                // Spawn particles
                Vector2 localPos = gameObject.transform.localPosition;
                Vector2 hitObjectPos = collision.transform.position;
                Bounds hitObjectBounds = collision.bounds;

                Vector3 spawnPos = Vector3.zero;
                float waterSurfaceY = transform.position.y + (_water.height / 2); // Assuming water is centered
                if (collision.transform.position.y >= _edgeCol.points[1].y + _edgeCol.offset.y + localPos.y)
                {
                    // Hit from above
                    spawnPos = hitObjectPos - new Vector2(0f, hitObjectBounds.extents.y);
                }
                else
                {
                    // Hit from below
                    spawnPos = hitObjectPos + new Vector2(0f, hitObjectBounds.extents.y);
                }

                Instantiate(_splashParticles, spawnPos, Quaternion.identity);

                // Clamp splash point to a MAX velocity
                int multiplier = 1;
                if (rb.velocity.y < 0)
                {
                    multiplier = -1;
                }
                else
                {
                    multiplier = 1;
                }

                float vel = rb.velocity.y * _water.forceMultiplier;
                vel = Mathf.Clamp(Mathf.Abs(vel), 0f, _water.MaxForce);
                vel *= multiplier;

                // _water.Splash(collision, vel);
                collision.gameObject.SendMessage("OnEnterWater", SendMessageOptions.DontRequireReceiver);
            }

        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((_waterMask.value & (1 << collision.gameObject.layer)) > 0)
        {
            collision.gameObject.SendMessage("OnExitWater", SendMessageOptions.DontRequireReceiver);
        }
    }
}
