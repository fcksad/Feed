using System.Collections.Generic;
using UnityEngine;

public interface IGrabbable
{
    Transform Transform { get; }
    Rigidbody Rigidbody { get; }
    List<Collider> GetColliders();
    bool SetDefaultPos {  get; }
    bool ToggleCollider { get ; }
    void InteractWith(IInteractable target);
}
    