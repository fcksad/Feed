using System.Collections;
using TMPro;
using UnityEngine;

public class FpsCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _fpsText;

    private float _deltaTime = 0;

    private void Start()
    {
        StartCoroutine(UpdateFps());
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
