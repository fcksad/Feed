using System.Collections;
using UnityEngine;

public class Pot : GrabbableObject
{
    [Header("Ground")]
    [SerializeField] private Renderer _groundRenderer;
    [SerializeField] private Material _materialToChange;
    [SerializeField] private Color _watering;
    [SerializeField] private Color _notWatering;
    [SerializeField] private float _lerpDuration = 1.0f;

    private Coroutine _colorLerpCoroutine;
    private int _materialIndex = -1;

    private void Awake()
    {
        var materials = _groundRenderer.materials;
        for (int i = 0; i < materials.Length; i++)
        {
            if (materials[i].shader == _materialToChange.shader &&
                materials[i].mainTexture == _materialToChange.mainTexture)
            {
                _materialIndex = i;
                break;
            }
        }
    }

    public override void Interact() { }

    public override void InteractWith(IInteractable target) { }

    public override void ReceiveInteractionFrom(IGrabbable item)
    {
        if (item is WateringCan)
        {
            StartWatering();
        }
    }

    private void StartWatering()
    {
        if (_materialIndex == -1) return;

        if (_colorLerpCoroutine != null)
            StopCoroutine(_colorLerpCoroutine);

        Color from = _groundRenderer.materials[_materialIndex].color;
        _colorLerpCoroutine = StartCoroutine(LerpColor(from, _watering));
    }

    public void StopWatering()
    {
        if (_materialIndex == -1) return;

        if (_colorLerpCoroutine != null)
            StopCoroutine(_colorLerpCoroutine);

        Color from = _groundRenderer.materials[_materialIndex].color;
        _colorLerpCoroutine = StartCoroutine(LerpColor(from, _notWatering));
    }

    private IEnumerator LerpColor(Color from, Color to)
    {
        float elapsed = 0f;
        while (elapsed < _lerpDuration)
        {
            elapsed += Time.deltaTime;
            Color current = Color.Lerp(from, to, elapsed / _lerpDuration);
            SetGroundColor(current);
            yield return null;
        }
        SetGroundColor(to);
    }

    private void SetGroundColor(Color color)
    {
        var materials = _groundRenderer.materials;
        materials[_materialIndex].color = color;
        _groundRenderer.materials = materials;
    }
}
