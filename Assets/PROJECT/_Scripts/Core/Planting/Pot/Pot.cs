using System.Collections.Generic;
using UnityEngine;

public class Pot : MonoBehaviour , IInteractable, IGrabbable
{
    [SerializeField] private Outline _outline;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private bool _setDefaultPos;
    [SerializeField] private bool _toggleCollider = true;
    [SerializeField] private List<Collider> _colliders = new List<Collider>();

    public Outline Outline => _outline;
    public Transform Transform => transform;
    public Rigidbody Rigidbody => _rb;
    public bool ToggleCollider => _toggleCollider;

    public bool SetDefaultPos { get => _setDefaultPos; }

    public List<Collider> GetColliders()
    {
        return _colliders;
    }

    public void Interact()
    {

    }

    public void InteractWith(IInteractable target)
    {
        
    }

    public void ReceiveInteractionFrom(IGrabbable item)
    {
       /* if (item is SeedBag)
        {
            Debug.Log("—емена посе€ны в горшок!");
            // тут можно создать растение и убрать предмет из рук
        }
        else
        {
            Debug.Log("Ётот предмет нельз€ использовать на горшке.");
        }*/
    }
}
