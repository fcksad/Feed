using System.Collections.Generic;
using UnityEngine.InputSystem;
using Zenject;

namespace Service
{
    public class HintService : IHintService
    {
        private HintView _hintView;
        private PlayerInput _playerInput;
        private IInputService _inputService;

        [Inject]
        public void Construct(HintView hintView, IInputService inputService, PlayerInput playerInput)
        {
            _hintView = hintView;
            _inputService = inputService;
            _playerInput = playerInput;
        }

        public void Initialize() { }

        public void ShowHint(string localizationAction, List<CharacterAction> actions)
        {
            var deviceType = GetDeviceType(_playerInput.currentControlScheme);

            List<string> keys = new List<string>();
            foreach (var action in actions)
            {
                keys.Add(_inputService.GetActionKey(action, _playerInput.currentControlScheme));
            }
            _hintView.Show(localizationAction, actions, keys, deviceType);
        }

        public void HideHint(string localizationAction)
        {
            _hintView.Hide(localizationAction);
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
   
