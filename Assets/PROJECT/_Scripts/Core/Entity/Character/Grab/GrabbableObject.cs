using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrabbableObject : MonoBehaviour, IGrabbable
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private bool _setDefaultPos;
    [SerializeField] private bool _toggleCollider = false;
    [SerializeField] private List<Collider> _colliders = new List<Collider>();
    public Transform Transform => transform;
    public Rigidbody Rigidbody => _rigidbody;
    public bool SetDefaultPos => _setDefaultPos;
    public bool ToggleCollider => _toggleCollider;

    public List<Collider> GetColliders()
    {
        return _colliders;
    }
    public void InteractWith(IInteractable target)
    {
    }
}
