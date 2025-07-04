using UnityEngine;
using Zenject;

public class DoorBell : InteractableObject
{
    [SerializeField] private Transform _soundPoint;
    [SerializeField] private AudioConfig _doorBell;

    private IAudioService _audioService;

    [Inject]
    public void Construct(IAudioService audioService)
    {
        _audioService = audioService;
    }

    public override void Interact()
    {
        _audioService.Play(_doorBell, position: _soundPoint.position);

    }
}
