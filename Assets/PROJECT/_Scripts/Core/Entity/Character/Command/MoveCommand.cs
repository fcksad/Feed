using UnityEngine;

public class MoveCommand : ICommand
{
    private readonly UnityEngine.CharacterController _controller;
    private readonly Transform _cameraTransform;
    private readonly Vector2 _input;
    private readonly float _speed;
    private bool _executed;

    public MoveCommand(UnityEngine.CharacterController controller, Transform cameraTransform, Vector2 input, float speed)
    {
        _controller = controller;
        _cameraTransform = cameraTransform;
        _input = input;
        _speed = speed;
    }

    public void Execute()
    {
        Vector3 forward = _cameraTransform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = _cameraTransform.right;
        right.y = 0;
        right.Normalize();

        Vector3 direction = right * _input.x + forward * _input.y;
        _controller.Move(direction * _speed * Time.deltaTime);
        _executed = true;
    }

    public bool IsFinished => _executed;
}
