using UnityEngine;
using UnityEngine.Localization;
using Zenject;

public class DoorBell : MonoBehaviour, IInteractable
{
    [SerializeField] protected LocalizedString _localizedString;

    [SerializeField] private Transform _soundPoint;

    protected string _name;
    public string Name => _name.ToString();

    [SerializeField] private AudioConfig _doorBell;

    private IAudioService _audioService;

    [Inject]
    public void Construct(IAudioService audioService)
    {
        _audioService = audioService;
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
        _audioService.Play(_doorBell, position: _soundPoint.position);

    }

    public void ReceiveInteractionFrom(IGrabbable item)
    {

    }
}
