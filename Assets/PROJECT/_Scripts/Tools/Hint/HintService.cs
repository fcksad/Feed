
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Zenject;

namespace Service
{
    public class HintService : IHintService
    {
        private HintView _hintView;
        private IInputService _inputService;
        private PlayerInput _playerInput;

        [Inject]
        public void Construct(HintView hintView, IInputService inputService, PlayerInput playerInput)
        {
            _hintView = hintView;
            _inputService = inputService;
            _playerInput = playerInput;
        }

        public void Initialize() { }

        public void ShowHint(List<CharacterAction> actions)
        {
            var deviceType = GetDeviceType(_playerInput.currentControlScheme);

            foreach (var action in actions)
            {
                string key = _inputService.GetActionKey(action, _playerInput.currentControlScheme);
                _hintView.Show(action, key, deviceType);
            }
        }

        public void HideHint(List<CharacterAction> actions)
        {
            foreach (var action in actions)
            {
                _hintView.Hide(action);
            }
        }

        public void HideAll()
        {
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
    }
}
   
