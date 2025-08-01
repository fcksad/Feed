using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Service
{
    public class HintService : IHintService, IInitializable, IDisposable
    {
        private HintView _hintView;
        private PlayerInput _playerInput;
        private IInputService _inputService;
        private ISaveService _saveService;
        private IControlsService _controlsService;

        private readonly Dictionary<string, List<string>> _cachedKeys = new();

        [Inject]
        public void Construct(HintView hintView, IInputService inputService, PlayerInput playerInput, ISaveService saveService, IControlsService controlsService)
        {
            _hintView = hintView;
            _inputService = inputService;
            _playerInput = playerInput;
            _saveService = saveService;
            _controlsService = controlsService;
        }

        public void Initialize() 
        {
            ToggleView(_saveService.SettingsData.HintData.IsEnable);
            _controlsService.OnBindingRebindEvent += HideAll;
        }

        public void Dispose()
        {
            _controlsService.OnBindingRebindEvent -= HideAll;
        }

        public void ShowHint(string localizationAction, List<CharacterAction> actions)
        {
            var controlScheme = _playerInput.currentControlScheme;
            var deviceType = GetDeviceType(controlScheme);
            string cacheKey = $"{localizationAction}_{deviceType}";

            if (!_cachedKeys.TryGetValue(cacheKey, out var keys))
            {
                keys = new List<string>(actions.Count);
                foreach (var action in actions)
                {
                    keys.Add(_inputService.GetActionKey(action, controlScheme));
                }

                _cachedKeys[cacheKey] = keys;
            }

            _hintView.Show(localizationAction, actions, keys, deviceType);
        }

        public void HideHint(string localizationAction)
        {
            _hintView.Hide(localizationAction);
        }

        public void HideAll()
        {
            _cachedKeys.Clear();
            _hintView.HideAll();
        }

        private ControlDeviceType GetDeviceType(string controlScheme)
        {
            return controlScheme switch
            {
                "Keyboard&Mouse" => ControlDeviceType.Keyboard,
                "Gamepad" => ControlDeviceType.Gamepad,
                _ => ControlDeviceType.Keyboard
            };
        }

        public void ToggleView(bool value)
        {
            _hintView.Toggle(value);
        }
    }
}
   
