using Service;
using UnityEngine;
using Zenject;

public class CharacterInput : ITickable
{
    private bool _isJumpHeld = false;
    private bool _isCrouching = false;

    private IInputService _inputService;
    private IControllable _controller;

    private Vector2 _moveInput = Vector2.zero;

    [Inject]
    public void Construct(IInputService inputService)
    {
        _inputService = inputService;
    }

    public void Initialize()
    {
        _inputService.AddActionListener(CharacterAction.Jump,onStarted: () => _isJumpHeld = true, onCanceled: () => _isJumpHeld = false);
        _inputService.AddActionListener(CharacterAction.Crouch, onStarted: Crouch);

        _inputService.ChangeInputMap(InputMapType.Player);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
    }

    public void Bind(IControllable controller)
    {
        _controller = controller;
    }

    public void Dispose()
    {
        _inputService.AddActionListener(CharacterAction.Jump, onStarted: () => _isJumpHeld = true, onCanceled: () => _isJumpHeld = false);
        _inputService.RemoveActionListener(CharacterAction.Crouch, onStarted: Crouch);
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
        _controller.Look(_inputService.GetVector2(CharacterAction.Look));

        Vector2 input = _inputService.GetVector2(CharacterAction.Move);
        input = input.normalized;

        bool run = _inputService.IsPressed(CharacterAction.Run) ? true : false;
        _controller.Move(input, run, _isJumpHeld, _isCrouching);
    }
}
