using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    private float vertical;
    private bool isFacingRight = true;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private KeyCode rightMoveKey;
    [SerializeField] private KeyCode leftMoveKey;
    [SerializeField] private KeyCode upMoveKey;
    [SerializeField] private KeyCode downMoveKey;
    [SerializeField] private float speed;
    private bool isMovementPrevented = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!isMovementPrevented)
        {
            horizontal = 0f;
            vertical = 0f;

            if (Input.GetKey(rightMoveKey))
            {
                horizontal = 1f;
            }
            else if (Input.GetKey(leftMoveKey))
            {
                horizontal = -1f;
            }

            if (Input.GetKey(upMoveKey))
            {
                vertical = 1f;
            }
            else if (Input.GetKey(downMoveKey))
            {
                vertical = -1f;
            }
            Flip();
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontal * speed, vertical * speed);
    }

    private bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    //Function that prevent from moving for x secionds
    public IEnumerator MovePrevent(float seconds)
    {
        float originSpeed = speed;
        speed = 0f;
        isMovementPrevented = true;

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        yield return new WaitForSeconds(seconds);
        speed = originSpeed;
        isMovementPrevented = false;

    }
}


