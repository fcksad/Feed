using UnityEngine;
using Zenject;

public class AudioButtonListener : CustomButton
{
    [SerializeField] private AudioConfig _soundConfig;

    private IAudioService _audioService;

    [Inject]
    public void Construct(IAudioService audioService)
    {
        _audioService = audioService;
    }

    private void Start()
    {
        Button.onClick.AddListener(PlaySound);
    }

    private void PlaySound()
    {
        _audioService.Play(_soundConfig);
    }
}
