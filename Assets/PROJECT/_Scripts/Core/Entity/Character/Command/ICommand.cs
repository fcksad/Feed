using UnityEngine;

public interface ICommand
{
    void Execute();
    bool IsFinished { get; }
}
   
