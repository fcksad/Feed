using System.Collections.Generic;
using UnityEngine;

public interface IGrabbable
{
    Transform Transform { get; }
    Rigidbody Rigidbody { get; }
    List<Collider> GetColliders();
    Quaternion LocalRotation { get; }
    bool SetDefaultPos {  get; }
    bool ToggleCollider { get ; }
    void OnGrab();
    void OnDrop();
}
    