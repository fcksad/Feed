using Service;
using System;
using UnityEngine;
using Zenject;

public class CharacterInput : IInitializable, IDisposable, ITickable
{
    private bool _isLocked = false;
    private bool _isJumpHeld = false;

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
    }

    public void Dispose()
    {
        _inputService.AddActionListener(CharacterAction.Jump, onStarted: () => _isJumpHeld = true, onCanceled: () => _isJumpHeld = false);
    }

    public void Lock(bool value)
    {
        _isLocked = value;
    }

    public void Tick()
    {
        if (_isLocked) return;

        _controller.Look(_inputService.GetVector2(CharacterAction.Look));

        Vector2 input = _inputService.GetVector2(CharacterAction.Move);
        input = input.normalized;

        bool run = _inputService.IsPressed(CharacterAction.Run) ? true : false;
        _controller.Move(input, run, _isJumpHeld);
    }
}
