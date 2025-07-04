using UnityEngine;
using Zenject;

public class Door : InteractableObject
{
    [SerializeField] private float _openAngle = 90f;      
    [SerializeField] private float _openSpeed = 2f;        
    [SerializeField] private bool _isOpen = false;
    [SerializeField] private Quaternion _closedRotation;
    [SerializeField] private Quaternion _openedRotation;

    private Coroutine _rotationCoroutine;

    [SerializeField] private AudioConfig _open;
    [SerializeField] private AudioConfig _close;
    private IAudioService _audioService;

    [Inject]
    public void Construct(IAudioService audioService)
    {
        _audioService = audioService;
    }

    public override void Interact()
    {
        if(_rotationCoroutine == null)
        {
            _rotationCoroutine = StartCoroutine(RotateDoor(!_isOpen));
        }
    }

    public void Open()
    {
        if(_isOpen)return;
        Interact();
    }

    private System.Collections.IEnumerator RotateDoor(bool open)
    {
        var config = open ? _open : _close;
        _audioService.Play(config, position: transform.position, parent: transform);

        Quaternion targetRotation = open ? _openedRotation : _closedRotation;
        Quaternion startRotation = transform.localRotation;
        float elapsed = 0f;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * _openSpeed;
            transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, elapsed);
            yield return null;
        }

        transform.localRotation = targetRotation;
        _isOpen = open;
        _rotationCoroutine = null;
    }

    private void OnValidate()
    {
        SetupRotations();
#if UNITY_EDITOR

        if (_isOpen)
        {
            transform.localRotation = _openedRotation;
        }
        else
        {
            transform.localRotation = _closedRotation;
        }
#endif
    }

    private void SetupRotations()
    {
        _closedRotation = Quaternion.identity;
        _openedRotation = _closedRotation * Quaternion.Euler(0f, _openAngle, 0f);
    }
}
