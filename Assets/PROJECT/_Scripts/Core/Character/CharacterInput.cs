using Service;
using System;
using UnityEngine;
using Zenject;

public class CharacterInput : IInitializable, IDisposable, ITickable
{
    private bool _isLocked = false;

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
        _inputService.AddActionListener(CharacterAction.Attack, onStarted: Attack);
    }

    public void Dispose()
    {
        _inputService.RemoveActionListener(CharacterAction.Attack, onStarted: Attack);
    }

    public void Lock(bool value)
    {
        _isLocked = value;
    }

    public void Tick()
    {
        if (_isLocked) return;

        Vector2 input = Vector2.zero;
        if (_inputService.IsPressed(CharacterAction.MoveUp)) input.y += 1;
        if (_inputService.IsPressed(CharacterAction.MoveDown)) input.y -= 1;
        if (_inputService.IsPressed(CharacterAction.MoveRight)) input.x += 1;
        if (_inputService.IsPressed(CharacterAction.MoveLeft)) input.x -= 1;

        input = input.normalized;

        if (input != Vector2.zero)
            _controller.Move(input);
    }

    private void Attack()
    {
        if (_isLocked) return;
        _controller.Attack();
    }
}
