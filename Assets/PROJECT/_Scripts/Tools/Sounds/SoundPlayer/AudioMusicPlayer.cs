using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class AudioMusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioConfig _musicConfig;
    private bool _isPlaying = true;
    private IAudioService _audioService;

    [Inject]
    private void Construct(IAudioService audioService)
    {
        _audioService = audioService;
    }

    public void Start()
    {
        _ = PlayLoop();
    }

    private async Task PlayLoop()
    {
        while (_isPlaying)
        {
            var source = _audioService.Play(_musicConfig, loop: false, fadeDuration: 1);

            if (source == null || source.clip == null)
                break;

            while (_isPlaying && source.isPlaying)
            {
                await Task.Delay(200);
            }
        }
    }

    private void OnDestroy()
    {
        _isPlaying = false;
        if (_audioService != null && _musicConfig != null)
        {
            _audioService.Stop(_musicConfig, fadeDuration: 1);
        }
    }
}
