using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

public enum MessageBoxType
{
    Ok,
    YesNo,
}

public struct MessageBoxCallback
{
    public string Text { get; set; }
    public int Value { get; set; }
}

public class MessageBoxController
{  
    private MessageBoxView _view;
    private UnityAction _onCancel;
    private UnityAction _onClose;
    private bool _isAccepted;

    [Inject]
    public void Construct(MessageBoxView view)
    {
        _view = view;
    }

    public void Signal(MessageBoxSignal signal)
    {
        UnityAction action = null;
        _view.YesButton.gameObject.SetActive(false);
        _view.NoButton.gameObject.SetActive(false);
        _view.OkButton.gameObject.SetActive(false);
        _onCancel = signal.OnCancel;
        _onClose = signal.OnClose;
        _isAccepted = false;
        switch (signal.TYpe)
        {
            case MessageBoxType.Ok:
                _view.OkButton.gameObject.SetActive(true);;
                _view.Text.gameObject.SetActive(true);
                _view.Text.text = signal.Message;
                _view.OkButton.interactable = true;
                action = () => {
                    _view.Background.onClick.AddListener(Close);
                    _view.OkButton.onClick.AddListener(() => { _isAccepted = true; signal.OnAccept?.Invoke(default); Close(); });
                };
                Open(action);
                break;
            case MessageBoxType.YesNo:
                _view.YesButton.gameObject.SetActive(true);
                _view.NoButton.gameObject.SetActive(true);
                _view.Text.gameObject.SetActive(true);
                _view.Text.text = signal.Message;
                _view.YesButton.interactable = true;
                action = () => {
                    _view.NoButton.onClick.AddListener(Close);
                    _view.Background.onClick.AddListener(Close);
                    _view.YesButton.onClick.AddListener(() => { _isAccepted = true; signal.OnAccept?.Invoke(default); Close(); });
                };
                Open(action);
                break;         
        }
    }

    private void Open(UnityAction action)
    {
        _view.gameObject.SetActive(true);
        Fade(1, 0.5f, action);
        _view.Background.GetComponent<Image>().DOFade(0.5f, 0.5f);
    }

    private void Close()
    {
        if (!_isAccepted)
            _onCancel?.Invoke();

        _onClose?.Invoke();
        _onCancel = null;
        _view.YesButton.onClick.RemoveAllListeners();
        _view.NoButton.onClick.RemoveAllListeners();
        _view.Background.onClick.RemoveAllListeners();
        Fade(0, 0.5f, () => _view.gameObject.SetActive(false));
        _view.Background.GetComponent<Image>().DOFade(0, 0.5f);
    }

    private void Fade(float endValue, float duration, UnityAction onComplete = null)
    {
        foreach (var grafic in _view.FadeGrafic)
        {
            grafic.DOFade(endValue, duration);
        }
        DOVirtual.DelayedCall(duration, () => onComplete?.Invoke());
    }
}
/*
var signal = new MessageBoxSignal
{
    TYpe = MessageBoxType.YesNo,
    Message = "Ты точно хочешь выйти?", // for localization create 
    OnAccept = (callback) =>
    {
        Debug.Log("Принято Yes!");
        // здесь логика, что делать, если пользователь нажал Да
    },
    OnCancel = () =>
    {
        Debug.Log("Отмена на выход.");
        // здесь логика, если пользователь отказался
    }
};

_messageBoxController.Signal(signal);*/