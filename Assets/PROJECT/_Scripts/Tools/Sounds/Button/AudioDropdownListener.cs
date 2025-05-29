using TMPro;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(TMP_Dropdown))]
public class AudioDropdownListener : MonoBehaviour
{
    [SerializeField] private AudioConfig _soundConfig;
    [SerializeField] private TMP_Dropdown _dropDownListen;

    private IAudioService _audioService;

    [Inject]
    public void Construct(IAudioService audioService)
    {
        _audioService = audioService;
    }

    private void Start()
    {
        _dropDownListen.onValueChanged.AddListener(_ => PlaySound());
    }

    private void PlaySound()
    {
        _audioService.Play(_soundConfig);
    }

    private void OnValidate()
    {
        if (_dropDownListen == null)
        {
            _dropDownListen = GetComponent<TMP_Dropdown>();
        }
    }
}
