using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrabbableObject : InteractableObject, IGrabbable
{

    [SerializeField] protected bool _setDefaultPos;
    [SerializeField] protected bool _toggleCollider = false;

    public bool SetDefaultPos => _setDefaultPos;
    public bool ToggleCollider => _toggleCollider;

    public virtual void OnGrab() { }
    public virtual void OnDrop() { }
}
