using System.Collections;
using TMPro;
using UnityEngine;
using Zenject;

public class FpsCounter : MonoBehaviour, IInitializable
{
    [SerializeField] private TextMeshProUGUI _fpsText;

    private float _deltaTime = 0;
    private Coroutine _coroutine;

    private ISaveService _saveService;

    [Inject]
    public void Construct(ISaveService saveService)
    {
        _saveService = saveService;
    }

    public void Initialize()
    {
        Toggle(_saveService.SettingsData.FPSData.IsEnable);
        _coroutine = StartCoroutine(UpdateFps());
    }

    public void Toggle(bool value)
    {
        gameObject.SetActive(value);
        if (value)
        {
            if (_coroutine == null)
            {
                _coroutine = StartCoroutine(UpdateFps());
            }
        }
        else
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
        }
    }

    private void Update()
    {
        _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
    }

    private IEnumerator UpdateFps()
    {
        while (true)
        {
            float fps = 1.0f / _deltaTime;
            _fpsText.text = $"FPS: {Mathf.CeilToInt((int)fps)}";
            yield return new WaitForSeconds(0.5f);
        }
    }
}
