using UnityEngine;

public class MoveCommand : ICharacterCommand
{
    private readonly UnityEngine.CharacterController _controller;
    private readonly Vector2 _input;
    private readonly float _speed;
    private bool _executed;

    public MoveCommand(UnityEngine.CharacterController controller, Vector2 input, float speed)
    {
        _controller = controller;
        _input = input;
        _speed = speed;
    }

    public void Execute()
    {
        Vector3 direction = new Vector3(_input.x, 0, _input.y);
        _controller.Move(direction * _speed * Time.deltaTime);
        _executed = true;
    }

    public bool IsFinished => _executed;
}
