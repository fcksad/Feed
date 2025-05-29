using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MessageBoxSignal
{
    public MessageBoxType TYpe;
    public string Message;
    internal UnityAction<MessageBoxCallback> OnAccept;
    public UnityAction OnCancel;
    public UnityAction OnClose;
    public string Input;
    public int minLength = 3;
    public MessageBoxSignal()
    {

    }

    public MessageBoxSignal(
        MessageBoxType type,
        string message,
        UnityAction<MessageBoxCallback> onAccept = null,
        UnityAction onCancel = null, UnityAction onClose = null,
        List<string> dropdownOptions = null)
    {
        this.TYpe = type;
        this.Message = message;
        this.OnAccept = onAccept;
        this.OnCancel = onCancel;
        this.OnClose = onClose;
    }
}
