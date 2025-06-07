using Service;
using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Zenject;

public class CharacterInput : IInitializable, IDisposable, ITickable
{
    private bool _isLocked = false;
    private bool _isJumpHeld = false;
    private bool _isCrouching = false;

    private IInputService _inputService;
    private IControllable _controller;

    private Vector2 _moveInput = Vector2.zero;

    [Inject]
    public void Construct(IInputService inputService, IControllable controller)
    {
        _inputService = inputService;
        _controller = controller;
    }

    public void Initialize()
    {
        _inputService.AddActionListener(CharacterAction.Jump,onStarted: () => _isJumpHeld = true, onCanceled: () => _isJumpHeld = false);
        _inputService.AddActionListener(CharacterAction.Crouch, onStarted: Crouch);

    }

    public void Dispose()
    {
        _inputService.AddActionListener(CharacterAction.Jump, onStarted: () => _isJumpHeld = true, onCanceled: () => _isJumpHeld = false);
        _inputService.RemoveActionListener(CharacterAction.Crouch, onStarted: Crouch);
    }

    public void Lock(bool value)
    {
        _isLocked = value;
    }

    private void Crouch()
    {
        bool targetState = !_isCrouching;

        if (_controller.Crouch(targetState))
        {
            _isCrouching = targetState;
        }
    }

    public void Tick()
    {
        if (_isLocked) return;

        _controller.Look(_inputService.GetVector2(CharacterAction.Look));

        Vector2 input = _inputService.GetVector2(CharacterAction.Move);
        input = input.normalized;

        bool run = _inputService.IsPressed(CharacterAction.Run) ? true : false;
        _controller.Move(input, run, _isJumpHeld, _isCrouching);
    }
}
