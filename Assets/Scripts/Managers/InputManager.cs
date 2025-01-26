using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public Vector2 Movement;
    public bool DashWasPressed;
    public bool AttackWasPressed;
    public bool Oxygen;

    private InputAction _moveAction;
    private InputAction _dashAction;
    private InputAction _attackAction;
    private InputAction _oxygenAction;

    private void Awake()
    {
        var playerInput = GetComponent<PlayerInput>();
        _moveAction = playerInput.actions["Move"];
        _dashAction = playerInput.actions["Dash"];
        _attackAction = playerInput.actions["Attack"];
        _oxygenAction = playerInput.actions["PassOxygen"];
    }

    private void Update()
    {
        Movement = _moveAction.ReadValue<Vector2>();
        
        DashWasPressed = _dashAction.WasPressedThisFrame();
        
        AttackWasPressed = _attackAction.IsPressed();
        
        Oxygen = _oxygenAction.WasPressedThisFrame();
        
        // Debug.Log("Movement Input: " + Movement);
    }
}