using Service;
using UnityEngine;
using Zenject;

public class FlashlightController : MonoBehaviour
{
    [SerializeField] private AudioConfig _on;
    [SerializeField] private AudioConfig _off;

    [SerializeField] private Transform _cameraObject;
    [SerializeField] private Transform _flashlightLight;
    private Vector3 _offset;

    [SerializeField] private float _speed = 3f;

    private bool _isOn = true;

    private IInputService _inputService;
    private IAudioService _audioService;

    [Inject]
    private void Constrcut(IInputService inputService, IAudioService audioService)
    {
        _inputService = inputService;
        _audioService = audioService;
    }

    private void Awake()
    {
        _offset = _flashlightLight.transform.position - _cameraObject.transform.position;
    }

    public void Initialize()
    {
        _inputService.AddActionListener(CharacterAction.Flashlight, onStarted: ToggleFlashlight);
    }

    public void Dispose()
    {
        _inputService.RemoveActionListener(CharacterAction.Flashlight, onStarted: ToggleFlashlight);
    }

    private void FixedUpdate()
    {
        _flashlightLight.transform.position = _cameraObject.position + _offset;
        _flashlightLight.transform.rotation = Quaternion.Slerp(_flashlightLight.transform.rotation, _cameraObject.rotation, _speed * Time.fixedDeltaTime);
    }

    private void ToggleFlashlight()
    {
        _isOn = !_isOn;
        var config = _isOn ? _on : _off;
        _flashlightLight.gameObject.SetActive(_isOn);
        _audioService.Play(config);
            
    }
}
