using System.Collections.Generic;
using UnityEngine;

public interface IUsable
{
    Transform Transform { get; }
    Rigidbody Rigidbody { get; }
    List<Collider> GetColliders();
    Vector3 LocalPosition { get; }  
    Quaternion LocalRotation { get; }
    void Use();
}