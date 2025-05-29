using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class PopupService : IPopupService, IInitializable
{
    private List<PopupElement> _activePopups = new List<PopupElement>();

    private PopupView _popupView;
    private IAudioService _audioService;
    private ISceneService _sceneSerivce;
    private IInstantiateFactoryService _instantiateFactoryService;

    [Inject]
    public PopupService(IAudioService audioService, PopupView popupView, ISceneService sceneSerivce, IInstantiateFactoryService instantiateFactoryService)
    {
        _audioService = audioService;
        _popupView = popupView;
        _sceneSerivce = sceneSerivce;
        _instantiateFactoryService = instantiateFactoryService;
    }

    public void Initialize()
    {
        _sceneSerivce.OnSceneUnloadEvent += ClearAll;
    }

    public void Show(Sprite sprite = null, string text = null, Vector3? position = null, float size = 1, float duration = 1.5f, float delay = 1f, float moveSpeed = 1f) //todo for text => have rect transform is problem :(
    {
        PopupElement prefab = sprite != null ? _popupView.ImagePrefab : _popupView.TextPrefab;

        PopupElement popupInstance = _instantiateFactoryService.Create(prefab, _popupView.Parent, position, Quaternion.identity);
        _activePopups.Add(popupInstance);

        if (sprite != null)
        {
            popupInstance.SpriteRenderer.sprite = sprite;
            popupInstance.transform.localScale = Vector3.one * size;
        }
        else if (text != null)
        {
            popupInstance.Text.SetText(text);
            popupInstance.Text.fontSize = size;
            if (position.HasValue)
                popupInstance.TextRect.anchoredPosition3D = position.Value;
            else
                popupInstance.TextRect.anchoredPosition3D = Vector3.zero;
        }

        _audioService.Play(_popupView.PopupAudio);
        MoveAndFade(popupInstance, duration, delay, moveSpeed);
    }

    public void ClearAll()
    {
        _activePopups.RemoveAll(p => p == null);

        foreach (var popup in _activePopups)
        {
            _instantiateFactoryService.Release(popup);
        }
        _activePopups.Clear();
    }

    private void MoveAndFade(PopupElement popup, float duration, float delay, float moveSpeed)
    {
        float totalLifetime = duration + delay;
        float moveDistance = moveSpeed * totalLifetime;
        Vector3 targetPos = popup.transform.position + Vector3.up * moveDistance;

        popup.transform.DOMoveY(targetPos.y, totalLifetime).SetEase(Ease.OutQuad);

        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(delay);

        bool faded = false;

        CanvasGroup canvasGroup = popup.GetComponent<CanvasGroup>();
        if (canvasGroup == null && popup.GetComponentInChildren<TextMeshProUGUI>() != null)
        {
            canvasGroup = popup.gameObject.AddComponent<CanvasGroup>();
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            sequence.Append(canvasGroup.DOFade(0f, duration));
            faded = true;
        }

        var spriteRenderer = popup.GetComponent<SpriteRenderer>();
        if (!faded && spriteRenderer != null)
        {
            sequence.Append(spriteRenderer.DOFade(0f, duration));
            faded = true;
        }

        sequence.OnComplete(() =>
        {
            _activePopups.Remove(popup);
            _instantiateFactoryService.Release(popup);
        });
    }
}
