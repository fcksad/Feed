using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

public enum MessageBoxType
{
    Ok,
    YesNo,
}

public class MessageBoxController
{
    private MessageBoxView _view;
    private UnityAction _onCancel;
    private UnityAction _onClose;
    private bool _isAccepted;
    private Tween _autoCloseTween;

    [Inject]
    public void Construct(MessageBoxView view)
    {
        _view = view;
    }

    public void ShowOk(string message, UnityAction onOk = null, UnityAction onClose = null, float autoCloseDelay = -1f)
    {
        SetupUI(MessageBoxType.Ok, message);

        _onCancel = null;
        _onClose = onClose;
        _isAccepted = false;

        _view.OkButton.onClick.AddListener(() =>
        {
            _isAccepted = true;
            onOk?.Invoke();
            Close();
        });

        _view.Background.onClick.AddListener(Close);
        Open();

        if (autoCloseDelay > 0)
            StartAutoClose(autoCloseDelay);
    }

    public void ShowYesNo(string message, UnityAction onYes, UnityAction onCancel = null, UnityAction onClose = null, float autoCloseDelay = -1f)
    {
        SetupUI(MessageBoxType.YesNo, message);

        _onCancel = onCancel;
        _onClose = onClose;
        _isAccepted = false;

        _view.YesButton.onClick.AddListener(() =>
        {
            _isAccepted = true;
            onYes?.Invoke();
            Close();
        });

        _view.NoButton.onClick.AddListener(Close);
        _view.Background.onClick.AddListener(Close);
        Open();

        if (autoCloseDelay > 0)
            StartAutoClose(autoCloseDelay);
    }

    private void SetupUI(MessageBoxType type, string message)
    {
        CancelAutoClose();

        _view.YesButton.gameObject.SetActive(false);
        _view.NoButton.gameObject.SetActive(false);
        _view.OkButton.gameObject.SetActive(false);
        _view.Text.gameObject.SetActive(true);
        _view.Text.text = message;

        _view.YesButton.onClick.RemoveAllListeners();
        _view.NoButton.onClick.RemoveAllListeners();
        _view.OkButton.onClick.RemoveAllListeners();
        _view.Background.onClick.RemoveAllListeners();

        if (type == MessageBoxType.Ok)
        {
            _view.OkButton.gameObject.SetActive(true);
            _view.OkButton.interactable = true;
        }
        else if (type == MessageBoxType.YesNo)
        {
            _view.YesButton.gameObject.SetActive(true);
            _view.NoButton.gameObject.SetActive(true);
            _view.YesButton.interactable = true;
        }
    }

    private void Open()
    {
        _view.gameObject.SetActive(true);
        Fade(1, 0.5f);
        _view.Background.GetComponent<Image>().DOFade(0.5f, 0.5f);
    }

    private void Close()
    {
        CancelAutoClose();

        if (!_isAccepted)
            _onCancel?.Invoke();

        _onClose?.Invoke();

        _view.YesButton.onClick.RemoveAllListeners();
        _view.NoButton.onClick.RemoveAllListeners();
        _view.OkButton.onClick.RemoveAllListeners();
        _view.Background.onClick.RemoveAllListeners();

        Fade(0, 0.5f, () => _view.gameObject.SetActive(false));
        _view.Background.GetComponent<Image>().DOFade(0, 0.5f);
    }

    private void Fade(float endValue, float duration, UnityAction onComplete = null)
    {
        foreach (var grafic in _view.FadeGrafic)
            grafic.DOFade(endValue, duration);

        DOVirtual.DelayedCall(duration, () => onComplete?.Invoke());
    }

    private void StartAutoClose(float delay)
    {
        _autoCloseTween = DOVirtual.DelayedCall(delay, () =>
        {
            if (!_isAccepted)
                Close();
        });
    }

    private void CancelAutoClose()
    {
        if (_autoCloseTween?.IsActive() == true)
            _autoCloseTween.Kill();
    }
}
