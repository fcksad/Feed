using UnityEngine;

public class MoveCommand : ICommand
{
    private readonly UnityEngine.CharacterController _controller;
    private readonly Transform _cameraTransform;
    private readonly Vector2 _input;
    private readonly float _speed;
    private readonly float _verticalVelocity;
    private bool _executed;

    public MoveCommand(UnityEngine.CharacterController controller, Transform cameraTransform, Vector2 input, float speed, float verticalVelocity)
    {
        _controller = controller;
        _cameraTransform = cameraTransform;
        _input = input;
        _speed = speed;
        _verticalVelocity = verticalVelocity;
    }

    public void Execute()
    {
        Vector3 forward = _cameraTransform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = _cameraTransform.right;
        right.y = 0;
        right.Normalize();

        Vector3 horizontal = (right * _input.x + forward * _input.y) * _speed;
        Vector3 move = horizontal;
        move.y = _verticalVelocity;

        _controller.Move(move * Time.deltaTime);
        _executed = true;
    }

    public bool IsFinished => _executed;
}
