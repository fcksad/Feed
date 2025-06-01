using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrabbableObject : MonoBehaviour, IGrabbable, IInteractable
{
    [SerializeField] protected Outline _outline;
    [SerializeField] protected Rigidbody _rigidbody;
    [SerializeField] protected Quaternion _rotate;
    [SerializeField] protected bool _setDefaultPos;
    [SerializeField] protected bool _toggleCollider = false;
    [SerializeField] protected List<Collider> _colliders = new List<Collider>();
    public Transform Transform => transform;
    public Rigidbody Rigidbody => _rigidbody;
    public bool SetDefaultPos => _setDefaultPos;
    public bool ToggleCollider => _toggleCollider;
    public Outline Outline => _outline;
    public Quaternion Rotate => _rotate;

    public virtual List<Collider> GetColliders()
    {
        return _colliders;
    }

    public virtual void Interact() { }
    public virtual void InteractWith(IInteractable target) { }
    public virtual void ReceiveInteractionFrom(IGrabbable item) { }
    public virtual void OnGrab() { }
    public virtual void OnDrop() { }
}
