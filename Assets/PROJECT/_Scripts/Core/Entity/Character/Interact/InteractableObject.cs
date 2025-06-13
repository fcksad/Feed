using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

public class InteractableObject : MonoBehaviour, IInteractable
{
    [SerializeField] protected LocalizedString _localizedString;
    public string Name => _name;
    protected string _name;

    [SerializeField] protected Rigidbody _rigidbody;
    [SerializeField] protected List<Collider> _colliders = new List<Collider>();
    [SerializeField] protected Vector3 _position;
    [SerializeField] protected Quaternion _rotate;
    public Vector3 LocalPosition => _position;
    public Quaternion LocalRotation => _rotate;
    public Transform Transform => transform;
    public Rigidbody Rigidbody => _rigidbody;

    protected virtual void Start()
    {
        _localizedString.StringChanged += name =>
        {
            _name = name;
        };

        _localizedString.RefreshString();
    }

    public virtual List<Collider> GetColliders()
    {
        return _colliders;
    }

    public virtual void Interact() { }
}
