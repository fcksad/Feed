using System.Collections.Generic;
using System;
using UnityEngine;
using static InputHintConfig.InputHintEntry;

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
            [field: SerializeField] public string ControlName { get; set; }
            [field: SerializeField] public string ControlButton { get; set; } 
            [field: SerializeField] public Sprite Icon { get; set; }
        }
    }

    [SerializeField] private List<InputHintEntry> _hintEntries = new();
    private Dictionary<(ControlDeviceType, string), HintInputView> _hintDictionary;

    public void Initialize()
    {
        _hintDictionary = new();

        foreach (var entry in _hintEntries)
        {
            foreach (var hint in entry.HintInputViews)
            {
                var key = (entry.ControlDeviceType, hint.ControlButton);
                if (!_hintDictionary.ContainsKey(key))
                {
                    _hintDictionary.Add(key, hint);
                }
            }
        }
    }

    public HintInputView GetHint(ControlDeviceType deviceType, string controlButton)
    {
        if (_hintDictionary == null)
            Initialize();

        var key = (deviceType, controlButton.ToLower());
        return _hintDictionary[key];
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
            "f1", "f2", "f3", "f4", "f5", "f6", "f7", "f8", "f9", "f10", "f11", "f12",
            "leftButton", "rightButton", "middleButton"
        });

        AutoFillDevice(ControlDeviceType.Gamepad, new[]
        {
            "buttonSouth", "buttonEast", "buttonNorth", "buttonWest",
            "leftShoulder", "rightShoulder",
            "leftTrigger", "rightTrigger",
            "up", "down", "left", "right",
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
            string formattedName = char.ToUpper(button[0]) + button.Substring(1);

            entry.HintInputViews.Add(new InputHintEntry.HintInputView
            {
                ControlButton = button.ToLower(),
                ControlName = formattedName,
                Icon = null
            });
        }

        _hintEntries.Add(entry);
    }
#endif
}