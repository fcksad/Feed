using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class AudioService : IAudioService, IInitializable
{
    private readonly Dictionary<AudioType, float> _volumes = new();
    private readonly Dictionary<(AudioType, string), List<AudioSource>> _namedSources = new();
    private readonly Dictionary<AudioType, AudioSource> _oneShootSources = new();

    private GameObject _audioRoot;
    private ISaveService _saveService;

    [Inject]
    public AudioService(ISaveService saveService)
    {
        _saveService = saveService;
    }

    public void Initialize()
    {
        _audioRoot = new GameObject("[AudioService]");
        UnityEngine.Object.DontDestroyOnLoad(_audioRoot);

        foreach (AudioType type in Enum.GetValues(typeof(AudioType)))
        {
            float volume = _saveService.SettingsData.AudioData.SoundVolumes.TryGetValue(type.ToString(), out float loadedVolume) ? loadedVolume : 0.5f;
            _volumes[type] = volume;

            if (type != AudioType.SFX)
            {
                var src = CreateAudioSource(type.ToString() + "OneShoot");
                src.volume = volume;
                _oneShootSources[type] = src;
            }
        }
    }

    public AudioSource Play(AudioConfig audio, bool loop = false, int clipIndex = -1, Transform parent = null, Vector3? position = null, float fadeDuration = 0)
    {
        if (audio.OneShoot == true)
        {
            _oneShootSources[audio.Type].pitch = UnityEngine.Random.Range(audio.MinPitch, audio.MaxPitch);
            _oneShootSources[audio.Type].PlayOneShot(GetRandomClip(audio.AudioClips));
            return _oneShootSources[audio.Type];
        }

        var go = new GameObject($"{audio.Type}_{audio.AudioName}");
        if (parent != null)
        {
            go.transform.SetParent(parent, worldPositionStays: false);
        }
        else
        {
            go.transform.SetParent(_audioRoot.transform);
        }

        if (position.HasValue)
        {
            go.transform.position = position.Value;
        }

        var src = go.AddComponent<AudioSource>();
        SetupSource(src, audio, loop, clipIndex);
        src.Play();
        src.DOFade(_volumes[audio.Type], fadeDuration);

        var key = (audio.Type, audio.AudioName);

        if (audio.Type != AudioType.SFX)
        {
            if (!_namedSources.ContainsKey(key)) _namedSources[key] = new List<AudioSource>();
            _namedSources[key].Add(src);
        }

        var playTime = src.clip.length + 0.5f;

        if (loop == false)
        {
            TimerRemove(src, key, playTime);
            UnityEngine.Object.Destroy(go, playTime);
        }

        return src;
    }

    public void Stop(AudioConfig audio, float fade = 0)
    {
        foreach (var kv in _namedSources.Where(key =>
         key.Key.Item1 == audio.Type && (string.IsNullOrEmpty(audio.AudioName) || key.Key.Item2 == audio.AudioName)).ToList())
        {
            foreach (var source in kv.Value)
            {
                if (source == null) continue;

                source.DOFade(0, fade).OnComplete(() => UnityEngine.Object.Destroy(source.gameObject));
            }

            _namedSources.Remove(kv.Key);
        }
    }

    public void Pause(AudioConfig audio)
    {
        foreach (var kv in _namedSources)
        {
            if (kv.Key.Item1 == audio.Type && (audio.name == null || kv.Key.Item2 == audio.name))
                kv.Value.ForEach(s => s?.Pause());
        }
    }

    public void Resume(AudioConfig audio)
    {
        foreach (var kv in _namedSources)
        {
            if (kv.Key.Item1 == audio.Type && (audio.name == null || kv.Key.Item2 == audio.name))
                kv.Value.ForEach(s => s?.UnPause());
        }
    }

    public void SetVolume(AudioType type, float value)
    {
        _volumes[type] = value;
        _saveService.SettingsData.AudioData.SoundVolumes[type.ToString()] = value;

        if (_oneShootSources.TryGetValue(type, out var oneShoot)) oneShoot.volume = value;

        foreach (var source in _namedSources)
        {
            if (source.Key.Item1 == type)
            {
                source.Value.RemoveAll(named => named == null);
                foreach (var named in source.Value)
                {
                    if (named != null) named.volume = value;
                }
            }
        }
    }

    public float GetVolume(AudioType type) => _volumes.TryGetValue(type, out float volume) ? volume : 0.5f;

    private void SetupSource(AudioSource src, AudioConfig audio, bool loop, int clipIndex = -1)
    {
        src.loop = loop;
        src.playOnAwake = false;
        src.spatialBlend = audio.SpatialBlend;
        src.pitch = UnityEngine.Random.Range(audio.MinPitch, audio.MaxPitch);

        if (clipIndex >= 0) { src.clip = audio.AudioClips[clipIndex]; }
        else { src.clip = GetRandomClip(audio.AudioClips); }

        src.clip = GetRandomClip(audio.AudioClips);
        src.volume = _volumes[audio.Type];
    }

    private async void TimerRemove(AudioSource src, (AudioType, string) key, float delay)
    {
        await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(delay));
        if (_namedSources.TryGetValue(key, out var list))
        {
            list.Remove(src);
            if (list.Count == 0) _namedSources.Remove(key);
        }
    }

    private AudioSource CreateAudioSource(string name, Transform parent = null, Vector3? pos = null)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent != null ? parent : _audioRoot.transform);
        obj.transform.position = pos ?? Vector3.zero;

        var source = obj.AddComponent<AudioSource>();
        source.playOnAwake = false;
        return source;
    }

    private AudioClip GetRandomClip(List<AudioClip> clips) => clips[UnityEngine.Random.Range(0, clips.Count)];

}