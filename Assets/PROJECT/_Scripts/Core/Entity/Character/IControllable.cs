using UnityEngine;

public interface IControllable 
{
    void Move(Vector2 input, bool isRunning);
    void Attack();
    void Look(Vector2 delta);
    void Jump();

}
