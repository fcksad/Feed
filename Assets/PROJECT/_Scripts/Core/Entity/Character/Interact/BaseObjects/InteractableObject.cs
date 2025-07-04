using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

public class InteractableObject : MonoBehaviour, IInteractable
{
    [SerializeField] protected LocalizedString _localizedString;
    public string Name => _name;
    protected string _name;

    protected virtual void Start()
    {
        _localizedString.StringChanged += name =>
        {
            _name = name;
        };

        _localizedString.RefreshString();
    }

    public virtual void Interact() { }
}
