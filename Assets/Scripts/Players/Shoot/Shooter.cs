using UnityEngine;

public class Shooter : MonoBehaviour
{
    public GameObject boomerangPrefab;
    private bool boomerangActive = false;
    private InputManager inputManager;
    
    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        // _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (inputManager.AttackWasPressed && !boomerangActive)
        {
            ThrowBoomerang();
            boomerangActive = true;  // Ensure only one boomerang is active
        }
    }

    void ThrowBoomerang()
    {
        GameObject boomerang = Instantiate(boomerangPrefab, transform.position, Quaternion.identity);
        boomerang.GetComponent<Boomerang>().shooter = transform;
        boomerang.transform.forward = transform.forward; // Align forward direction
    }

    public void OnBoomerangReturn()
    {
        boomerangActive = false; // Allow throwing another boomerang
    }
}