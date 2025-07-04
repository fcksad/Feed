using Localization;
using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    [SerializeField] protected LocalizedName _localizedName;
    public string Name => _localizedName.Name;

    protected virtual void Start()
    {
        _localizedName.Init();
    }

    public virtual void Interact() { }

    protected virtual void Destroy()
    {
        _localizedName.Dispose();
    }
}
