using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[RequireComponent(typeof(Rigidbody))]
public class GrabbableObject : MonoBehaviour, IGrabbable, IInteractable
{
    [SerializeField] protected LocalizedString _localizedString;
    [SerializeField] protected Rigidbody _rigidbody;
    [SerializeField] protected Quaternion _rotate;
    [SerializeField] protected bool _setDefaultPos;
    [SerializeField] protected bool _toggleCollider = false;
    [SerializeField] protected List<Collider> _colliders = new List<Collider>();

    protected string _name;

    public Transform Transform => transform;
    public Rigidbody Rigidbody => _rigidbody;
    public bool SetDefaultPos => _setDefaultPos;
    public bool ToggleCollider => _toggleCollider;
    public string Name => _name;
    public Quaternion Rotate => _rotate;

    public virtual List<Collider> GetColliders()
    {
        return _colliders;
    }

    protected virtual void Start()
    {
        _localizedString.StringChanged += name =>
        {
            _name = name;
        };

        _localizedString.RefreshString();
    }

    public virtual void Interact() { }
    public virtual void InteractWith(IInteractable target) { }
    public virtual void ReceiveInteractionFrom(IGrabbable item) { }
    public virtual void OnGrab() { }
    public virtual void OnDrop() { }
}
