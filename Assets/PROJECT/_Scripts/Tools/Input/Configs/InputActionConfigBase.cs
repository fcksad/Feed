using System;
using UnityEngine;
using UnityEngine.InputSystem;


// make more ACTION variant
[CreateAssetMenu(fileName = "Action", menuName = "Configs/Input/Action")]
public class InputActionConfigBase : ScriptableObject, IInputAction
{
    [field: SerializeField] public CharacterAction Action { get; private set; }
    [field: SerializeField] public InputActionReference InputReference { get; private set; }

    public Action OnStartedEvent;
    public Action OnPerformedEvent;
    public Action OnCanceledEvent;

    public void Initialize()
    {
        InputReference.action.started += OnStarted;
        InputReference.action.performed += OnPerformed;
        InputReference.action.canceled += OnCanceled;
    }

    public void Cleanup()
    {
        InputReference.action.started -= OnStarted;
        InputReference.action.performed -= OnPerformed;
        InputReference.action.canceled -= OnCanceled;
    }

    private void OnStarted(InputAction.CallbackContext ctx)
    {
        OnStartedEvent?.Invoke();
    }

    private void OnPerformed(InputAction.CallbackContext ctx)
    {
        OnPerformedEvent?.Invoke();
    }

    private void OnCanceled(InputAction.CallbackContext ctx)
    {
        OnCanceledEvent?.Invoke();
    }

    public bool IsPressed()
    {
        return InputReference.action.IsPressed();
    }
    public string GetKeyName()
    {
        if (InputReference != null && InputReference.action != null)
        {
            return InputReference.action.GetBindingDisplayString();
        }

        return "";
    }
}
