using UnityEngine;

public interface IControllable 
{
    public void Move(Vector2 input, bool isRunning, bool jumpRequested);
    void Look(Vector2 delta);

}
