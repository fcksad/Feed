using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using Localization;
using Service;
using Zenject;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextButtonListener : MonoBehaviour
{
    [SerializeField] protected LocalizationConfig _localizationConfig;
    [SerializeField] private TextMeshProUGUI _targetText;

    private ILocalizationService _localizationService;

    [Inject]
    public void Construct(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
    }

    private void Awake()
    {
        if (_localizationConfig == null)
        {
            Debug.LogWarning($"Localization config not found -  ${gameObject.name}");
            return;
        }

        _localizationService.BindTo(_targetText, _localizationConfig, this);
    }

    private void OnValidate()
    {
        if (_targetText == null)
        {
            _targetText = GetComponent<TextMeshProUGUI>();
        }
    }
}
