using UnityEngine;

public class CharacterController : IControllable
{
    private readonly ICharacterCommandController _commandController;
    private readonly Character _character;

    private readonly float _moveSpeed = 5f;

    public CharacterController(ICharacterCommandController commandController, Character character)
    {
        _commandController = commandController;
        _character = character;
    }

    public void Move(Vector2 input)
    {
        var command = new MoveCommand(_character.CharacterController, input, _moveSpeed);
        _commandController.SetCommand(command);
    }

    public void Attack()
    {
        var command = new AttackCommand(_character);
        _commandController.SetCommand(command);
    }
}
