using UnityEngine;

public interface IEntityTickService
{

    public void Register(IEntityTickable tickable, float rate);
    public void Unregister(IEntityTickable tickable);


}
