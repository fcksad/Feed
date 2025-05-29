using UnityEngine;

public class AttackCommand : ICharacterCommand
{
    private readonly Character _character;
    private float _startTime;
    private float _duration = 0.5f;
    private bool _executed;

    public AttackCommand(Character character)
    {
        _character = character;
    }

    public void Execute()
    {
        //_character.PlayAttackAnimation();
        _startTime = Time.time;
        _executed = true;
    }

    public bool IsFinished => _executed && Time.time - _startTime >= _duration;
}
