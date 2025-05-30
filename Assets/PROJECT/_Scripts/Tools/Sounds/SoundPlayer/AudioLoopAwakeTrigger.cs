using UnityEngine;
using Zenject;

public class AudioLoopAwakeTrigger : MonoBehaviour
{
    [SerializeField] private AudioConfig _audioConfig;

    private IAudioService _audioService;

    [Inject]
    public void Construct(IAudioService audioService)
    {
        _audioService = audioService;
    }

    private void Start()
    {
        PlayLoop();
    }

    private void PlayLoop()
    {
        _audioService.Play(_audioConfig, true);
    }

    private void OnDestroy()
    {
        _audioService.Stop(_audioConfig);
    }
}
