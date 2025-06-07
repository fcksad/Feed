using UnityEngine;

public interface IControllable 
{
    void Move(Vector2 input, bool isRunning, bool jumpRequested, bool isCrouching);
    void Look(Vector2 delta);
    bool Crouch(bool isCrouching);

}
