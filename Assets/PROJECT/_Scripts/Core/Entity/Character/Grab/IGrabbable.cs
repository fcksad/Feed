using System.Collections.Generic;
using UnityEngine;

public interface IGrabbable
{
    Transform Transform { get; }
    Rigidbody Rigidbody { get; }
    List<Collider> GetColliders();
    Quaternion Rotate { get; }
    bool SetDefaultPos {  get; }
    bool ToggleCollider { get ; }
    void InteractWith(IInteractable target);
    void OnGrab();
    void OnDrop();
}
    