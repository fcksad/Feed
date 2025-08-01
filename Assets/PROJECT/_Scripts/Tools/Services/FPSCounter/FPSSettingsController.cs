using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class FPSSettingsController : MonoBehaviour
{
    [SerializeField] private Toggle _toggle;

    private ISaveService _saveService;
    private FpsCounter _fpsCounter;

    [Inject]
    public void Construct(ISaveService saveService, FpsCounter fpsCounter)
    {
        _saveService = saveService;
        _fpsCounter = fpsCounter;
    }

    private void OnEnable()
    {
        _toggle.isOn = _saveService.SettingsData.FPSData.IsEnable;
        _toggle.onValueChanged.AddListener(OnToggle);
    }

    private void OnDisable()
    {
        _toggle.onValueChanged.RemoveListener(OnToggle);
    }

    private void OnToggle(bool value)
    {
        _fpsCounter.Toggle(value);
        _saveService.SettingsData.FPSData.IsEnable = value;
    }
}
