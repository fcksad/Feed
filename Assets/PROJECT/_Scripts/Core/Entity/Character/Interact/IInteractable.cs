using System.Collections.Generic;
using UnityEngine;

public interface IInteractable 
{
    public string Name { get;}
    void Interact();
}

public interface IGrabbable
{
    void OnGrab(Transform pointToMove);
    void OnDrop();
}

public interface IUsable
{
    void Use();
    Transform Transform { get; }
    Rigidbody Rigidbody { get; }
    List<Collider> GetColliders();
    Vector3 LocalPosition { get; }
    Quaternion LocalRotation { get; }
}
