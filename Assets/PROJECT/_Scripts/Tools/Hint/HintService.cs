
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

        public void ShowHint(CharacterAction action)
        {
            string key = _inputService.GetActionKey(action);
            ControlDeviceType deviceType = _playerInput.currentControlScheme == "Gamepad" ? ControlDeviceType.Gamepad : ControlDeviceType.Keyboard;

            _hintView.Show(action, key.ToLowerInvariant(), deviceType);
        }

        public void HideHint(CharacterAction action)
        {
            _hintView.Hide(action);
        }

        public void HideAll()
        {
            _hintView.HideAll();
        }
    }
}
   
