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

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _runAction;
    private InputAction _dashAction;

    private void Awake()
    {
        var playerInput = GetComponent<PlayerInput>();
        _moveAction = playerInput.actions["Move"];
        _jumpAction = playerInput.actions["Jump"];
        _runAction = playerInput.actions["Run"];
        _dashAction = playerInput.actions["Dash"];
    }

    private void Update()
    {
        Movement = _moveAction.ReadValue<Vector2>();

        JumpWasPressed = _jumpAction.WasPressedThisFrame();
        JumpIsHeld = _jumpAction.IsPressed();
        JumpWasReleased = _jumpAction.WasReleasedThisFrame();

        RunIsHeld = _runAction.IsPressed();
        
        DashWasPressed = _dashAction.WasPressedThisFrame();
    }
}