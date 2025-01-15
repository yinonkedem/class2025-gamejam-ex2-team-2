using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public Vector2 Movement;
    public bool JumpWasPressed;
    public bool JumpIsHeld;
    public bool JumpWasReleased;
    public bool RunIsHeld;
    public bool DashWasPressed;
    public bool AttackWasPressed;  // Add for boomerang attack

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _runAction;
    private InputAction _dashAction;
    private InputAction _attackAction;  // Add for boomerang attack

    private void Awake()
    {
        var playerInput = GetComponent<PlayerInput>();
        _moveAction = playerInput.actions["Move"];
        _jumpAction = playerInput.actions["Jump"];
        _runAction = playerInput.actions["Run"];
        _dashAction = playerInput.actions["Dash"];
        _attackAction = playerInput.actions["Attack"];  // Add for boomerang attack
    }

    private void Update()
    {
        Movement = _moveAction.ReadValue<Vector2>();

        JumpWasPressed = _jumpAction.WasPressedThisFrame();
        JumpIsHeld = _jumpAction.IsPressed();
        JumpWasReleased = _jumpAction.WasReleasedThisFrame();

        RunIsHeld = _runAction.IsPressed();
        
        DashWasPressed = _dashAction.WasPressedThisFrame();
        AttackWasPressed = _attackAction.WasPressedThisFrame();  // Add for boomerang attack
    }
}