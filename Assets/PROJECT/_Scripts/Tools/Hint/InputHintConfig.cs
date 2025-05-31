using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "InputHintConfig", menuName = "Configs/Input/Input Hint Config")]
public class InputHintConfig : ScriptableObject
{
    [Serializable]
    public class InputHintEntry
    {
        [field: SerializeField] public ControlDeviceType ControlDeviceType { get; set; }
        [field: SerializeField] public List<HintInputView> HintInputViews { get; set; } = new();

        [Serializable]
        public class HintInputView
        {
            [field: SerializeField] public string ControlButton { get; set; } 
            [field: SerializeField] public Sprite Icon { get; set; }
        }
    }

    [SerializeField] private List<InputHintEntry> _hintEntries = new();
    private Dictionary<(ControlDeviceType, string), Sprite> _hintDictionary;

    public void Initialize()
    {
        _hintDictionary = new Dictionary<(ControlDeviceType, string), Sprite>();

        foreach (var entry in _hintEntries)
        {
            foreach (var hint in entry.HintInputViews)
            {
                var key = (entry.ControlDeviceType, hint.ControlButton);
                if (!_hintDictionary.ContainsKey(key))
                {
                    _hintDictionary.Add(key, hint.Icon);
                }
            }
        }
    }

    public Sprite GetIcon(ControlDeviceType deviceType, string controlButton)
    {
        if (_hintDictionary == null)
            Initialize();

        var key = (deviceType, controlButton);
        return _hintDictionary.TryGetValue(key, out var icon) ? icon : null;
    }


#if UNITY_EDITOR
    [ContextMenu("Auto Fill All Common Hints")]
    private void AutoFillDefaults()
    {
        _hintEntries.Clear();

        AutoFillDevice(ControlDeviceType.Keyboard, new[]
        {
            "q","w","e","r","t","y","u","i","o","p",
            "a","s","d","f","g","h","j","k","l",
            "z","x","c","v","b","n","m",
            "1","2","3","4","5","6","7","8","9","0",
            "space", "enter", "tab", "escape", "backspace",
            "leftShift", "rightShift", "leftCtrl", "rightCtrl",
            "leftAlt", "rightAlt",
            "upArrow", "downArrow", "leftArrow", "rightArrow",
            "f1", "f2", "f3", "f4", "f5", "f6", "f7", "f8", "f9", "f10", "f11", "f12"
        });

        AutoFillDevice(ControlDeviceType.Gamepad, new[]
        {
            "buttonSouth", "buttonEast", "buttonNorth", "buttonWest",
            "leftShoulder", "rightShoulder",
            "leftTrigger", "rightTrigger",
            "dpad/up", "dpad/down", "dpad/left", "dpad/right",
            "start", "select",
            "leftStickPress", "rightStickPress"
        });

        Debug.Log("InputHintConfig fully auto-filled with nested entries.");
    }

    private void AutoFillDevice(ControlDeviceType type, string[] buttons)
    {
        var entry = new InputHintEntry { ControlDeviceType = type };
        foreach (var button in buttons)
        {
            entry.HintInputViews.Add(new InputHintEntry.HintInputView
            {
                ControlButton = button,
                Icon = null
            });
        }

        _hintEntries.Add(entry);
    }
#endif
}