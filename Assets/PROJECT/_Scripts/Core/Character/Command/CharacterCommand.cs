using System.Data;
using UnityEngine;

public class CharacterCommand
{
    public CommandType CommandType;

    public Vector2 Direction;

    public bool IsComplete;

    public CharacterCommand(CommandType commandType, Vector2 direction)
    {
        CommandType = commandType;
        Direction = direction;
    }

    public CharacterCommand(CommandType commandType)
    {
        CommandType = commandType;
    }

}