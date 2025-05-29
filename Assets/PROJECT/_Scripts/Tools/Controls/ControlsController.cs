using Service;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using Zenject;

public class BindingReference
{
    public Control Control;
    public InputAction Action;
    public int BindingIndex;
    public Guid BindingId;

    public BindingReference(Control control, InputAction action, int bindingIndex)
    {
        Control = control;
        Action = action;
        BindingIndex = bindingIndex;
        BindingId = action.bindings[bindingIndex].id;
    }
}

public enum ControlDeviceType
{
    Keyboard,
    Mouse,
    Gamepad,
    Touchscreen
}

[Serializable]
public class ControlPage
{
    [field: SerializeField] public List<ControlDeviceType> Devices;
    [field: SerializeField] public GameObject Parent;
}

public class ControlsController : MonoBehaviour
{
    [SerializeField] private Control _bindingPrefab;
    [SerializeField] private GameObject _waitingForInputScreen;
    [SerializeField] private List<ControlPage> _controlPages = new List<ControlPage>();
    [SerializeField] private LocalizedString _localizedResetKey;
    [SerializeField] private LocalizedString _localizedBindKey;

    private InputActionMap _actionMap;

    private List<BindingReference> _bindings = new List<BindingReference>();

    private IControlService _controlsService;
    private ILocalizationService _localizationService;
    private MessageBoxController _messageBoxController;
    private DiContainer _container;

    private readonly Dictionary<ControlDeviceType, string> _deviceTags = new()
{
    { ControlDeviceType.Keyboard, "<Keyboard>" },
    { ControlDeviceType.Mouse, "<Mouse>" },
    { ControlDeviceType.Gamepad, "<Gamepad>" },
    { ControlDeviceType.Touchscreen, "<Touchscreen>" }
};

    [Inject]
    public void Construct(IControlService controlService, MessageBoxController  messageBoxController, ILocalizationService localizationService, DiContainer container)
    {
        _controlsService = controlService;
        _localizationService = localizationService;
        _messageBoxController = messageBoxController;
        _container = container;
    }

    private void Awake()
    {
        _actionMap = _controlsService.GetFirstActionMap(); //todo
        GenerateBindings();
    }

    private void GenerateBindings()
    {
        foreach (var controlPage in _controlPages)
        {

            foreach (var device in controlPage.Devices)
            {
                if (!_deviceTags.TryGetValue(device, out var deviceTag))
                {
                    Debug.LogWarning($"Device {device} not supported!");
                    continue;
                }

                foreach (var action in _actionMap.actions)
                {
                    for (int i = 0; i < action.bindings.Count; i++)
                    {
                        var binding = action.bindings[i];

                        if (binding.isComposite || binding.isPartOfComposite)
                            continue;

                        if (!binding.effectivePath.StartsWith(deviceTag))
                            continue;

                        GameObject newObj = _container.InstantiatePrefab(_bindingPrefab, controlPage.Parent.transform);
                        Control bindingUI = newObj.GetComponent<Control>();
                        var bindingRef = new BindingReference(bindingUI, action, i);
                        _bindings.Add(bindingRef);

                        UpdateBindingUI(bindingRef);

                        bindingUI.KeyBind.onClick.AddListener(() =>
                        {
                            StartRebind(bindingRef);
                        });

                        bindingUI.ResetButton.onClick.AddListener(() =>
                        {
                            ResetBinding(bindingRef);
                        });
                    }
                }
            }
        }

        CheckConflicts();
    }

    private void StartRebind(BindingReference bindingRef)
    {

        var signal = new MessageBoxSignal
        {
            TYpe = MessageBoxType.YesNo,
            Message = _localizedBindKey.GetLocalizedString(),
            OnAccept = (callback) =>
            {
                _waitingForInputScreen.SetActive(true);
                bindingRef.Control.KeyAction.text = "Press any key...";

                _controlsService.Binding(bindingRef.Action.name, bindingRef.BindingIndex, () =>
                {
                    _waitingForInputScreen.SetActive(false);
                    UpdateBindingUI(bindingRef);
                    CheckConflicts();
                });
            }
        };

        _messageBoxController.Signal(signal);
    }

    private void ResetBinding(BindingReference bindingRef)
    {
        var signal = new MessageBoxSignal
        {
            TYpe = MessageBoxType.YesNo,
            Message = _localizedResetKey.GetLocalizedString(),
            OnAccept = (callback) =>
            {
                _controlsService.Rebinding(bindingRef.Action, bindingRef.BindingId);
                UpdateBindingUI(bindingRef);
                CheckConflicts();
            }
        };

        _messageBoxController.Signal(signal);
    }

    private void UpdateBindingUI(BindingReference bindingRef)
    {
        var binding = bindingRef.Action.bindings[bindingRef.BindingIndex];
        bindingRef.Control.KeyAction.text = bindingRef.Action.name; 
        bindingRef.Control.KeyBind.GetComponentInChildren<TextMeshProUGUI>().text = binding.ToDisplayString();
    }

    private void CheckConflicts()
    {
        var usedBindings = new Dictionary<string, List<BindingReference>>();

        foreach (var bindingRef in _bindings)
        {
            var path = bindingRef.Action.bindings[bindingRef.BindingIndex].effectivePath;
            if (string.IsNullOrEmpty(path))
                continue;

            if (!usedBindings.ContainsKey(path))
                usedBindings[path] = new List<BindingReference>();

            usedBindings[path].Add(bindingRef);
        }

        foreach (var bindingRef in _bindings)
        {
            var path = bindingRef.Action.bindings[bindingRef.BindingIndex].effectivePath;
            bool hasConflict = usedBindings.ContainsKey(path) && usedBindings[path].Count > 1;

            var color = bindingRef.Control.Background.color;
            color.a = hasConflict ? 1f : 0f; 
            bindingRef.Control.Background.color = color;
        }
    }
}
