using UnityEngine;

public interface IAudioService
{
    void Play(AudioConfig audio, bool loop = false, Transform parent = null, Vector3? position = null, float fadeDuration = 0f);
    void Stop(AudioConfig audio, float fadeDuration = 0f); 
    void Pause(AudioConfig audio);
    void Resume(AudioConfig audio);
    void SetVolume(AudioType type, float value);
    float GetVolume(AudioType type);
}
