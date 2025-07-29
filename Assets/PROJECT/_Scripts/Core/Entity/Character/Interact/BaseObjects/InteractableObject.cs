using Localization;
using System;
using UnityEngine;
using Zenject;

public class InteractableObject : MonoBehaviour, IInteractable
{
    [SerializeField] protected LocalizationConfig _localizationConfig;
    public string Name { get => _localizationService.GetLocalizationString(_localizationConfig);}

    private ILocalizationService _localizationService;

    [Inject]
    public void Construct(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
    }

    public virtual void Interact() { }


}
