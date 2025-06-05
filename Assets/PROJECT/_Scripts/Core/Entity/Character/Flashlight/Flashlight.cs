using Service;
using UnityEngine;
using Zenject;

public class Flashlight : MonoBehaviour
{
    [SerializeField] private Light _flashlightLight;
    [SerializeField] private AudioConfig _off;
    [SerializeField] private AudioConfig _on;

    private IInputService _inputService;
    private IAudioService _audioService;

    private bool _isOn = false;

    [Inject]
    private void Constrcut(IInputService inputService, IAudioService audioService)
    {
        _inputService = inputService;
        _audioService = audioService;
    }

    private void Start()
    {
        _inputService.AddActionListener(CharacterAction.Flashlight, onStarted: ToggleFlashlight);
    }

    private void OnDestroy()
    {
        _inputService.RemoveActionListener(CharacterAction.Flashlight, onStarted: ToggleFlashlight);
    }

    private void ToggleFlashlight()
    {
        _isOn = !_isOn;
        var config = _isOn ? _on : _off;
        _flashlightLight.enabled = _isOn;
        _audioService.Play(config);

    }

}
