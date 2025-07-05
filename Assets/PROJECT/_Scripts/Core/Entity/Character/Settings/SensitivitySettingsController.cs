using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SensitivitySettingsController : MonoBehaviour
{
    private float _maxSensitivity = 1.0f;
    private float _minSensitivity = 0.1f;

    [field: SerializeField] private Slider _slider;

    private ISaveService _saveService;

    [Inject]
    public void Construct(ISaveService saveService)
    {
        _saveService = saveService;
    }

    private void Awake()
    {
        _slider.maxValue = _maxSensitivity;
        _slider.minValue = _minSensitivity;
    }

    private void OnEnable()
    {
        _slider.value = _saveService.SettingsData.CharacterSettingsData.Sensitivity;
        _slider.onValueChanged.AddListener(OnSliderChanged);
    }

    private void OnDisable()
    {
        _slider.onValueChanged.RemoveListener(OnSliderChanged);
    }

    private void OnSliderChanged(float value)
    {
        _saveService.SettingsData.CharacterSettingsData.Sensitivity = value;
        CharacterStaticData.MouseSensitivity = value;
    }
}
