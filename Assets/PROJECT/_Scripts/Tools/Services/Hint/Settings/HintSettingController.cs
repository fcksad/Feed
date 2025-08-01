using Service;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


public class HintSettingController : MonoBehaviour
{
    [SerializeField] private Toggle _toggle;

    private ISaveService _saveService;
    private IHintService _hintService;

    [Inject]
    public void Construct(ISaveService saveService, IHintService hintService)
    {
        _saveService = saveService;
        _hintService = hintService;
    }

    private void OnEnable()
    {
        _toggle.isOn = _saveService.SettingsData.HintData.IsEnable;
        _toggle.onValueChanged.AddListener(OnToggle);
    }

    private void OnDisable()
    {
        _toggle.onValueChanged.RemoveListener(OnToggle);
    }

    private void OnToggle(bool value)
    {
        _hintService.ToggleView(value);
        _saveService.SettingsData.HintData.IsEnable = value;
    }
}
