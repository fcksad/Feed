using UnityEngine;
using UnityEngine.Localization;
using Zenject;

public class Door : MonoBehaviour, IInteractable
{

    [SerializeField] protected LocalizedString _localizedString;
    [SerializeField] private float _openAngle = 90f;      
    [SerializeField] private float _openSpeed = 2f;        
    [SerializeField] private bool _startsOpen = false;
    [SerializeField] private Transform _pivot;

    protected string _name;
    public string Name => _name.ToString();

    private bool _isOpen;
    private Quaternion _closedRotation;
    private Quaternion _openedRotation;
    private Coroutine _rotationCoroutine;

    [SerializeField] private AudioConfig _open;
    [SerializeField] private AudioConfig _close;
    private IAudioService _audioService;

    [Inject]
    public void Construct(IAudioService audioService)
    {
        _audioService = audioService;
    }

    private void Awake()
    {
        if (_pivot == null)
            _pivot = transform;

        _closedRotation = _pivot.localRotation;
        _openedRotation = _closedRotation * Quaternion.Euler(0f, _openAngle, 0f);

        if (_startsOpen)
        {
            _pivot.localRotation = _openedRotation;
            _isOpen = true;
        }
    }

    protected virtual void Start()
    {
        _localizedString.StringChanged += name =>
        {
            _name = name;
        };

        _localizedString.RefreshString();
    }

    public void Interact()
    {
        if (_rotationCoroutine != null)
            StopCoroutine(_rotationCoroutine);

        _rotationCoroutine = StartCoroutine(RotateDoor(!_isOpen));
    }

    public void ReceiveInteractionFrom(IGrabbable item)
    {
        
    }

    private System.Collections.IEnumerator RotateDoor(bool open)
    {
        var config = open ? _open : _close;
        _audioService.Play(config, position: transform.position, parent: transform);

        Quaternion targetRotation = open ? _openedRotation : _closedRotation;
        Quaternion startRotation = _pivot.localRotation;
        float elapsed = 0f;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * _openSpeed;
            _pivot.localRotation = Quaternion.Slerp(startRotation, targetRotation, elapsed);
            yield return null;
        }

        _pivot.localRotation = targetRotation;
        _isOpen = open;
        _rotationCoroutine = null;
    }
}
