using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Service
{
    public class HintView : MonoBehaviour
    {
        [SerializeField] private InputHintConfig _inputHintConfig;
        [SerializeField] private HintImage _hintImage;
        [SerializeField] private Transform _hintHolder;

        private Dictionary<CharacterAction , HintImage > _hints = new();

        private InstantiateFactoryService _instantiateFactoryService;

        [Inject]
        public void Construct(InstantiateFactoryService instantiateFactoryService)
        {
            _instantiateFactoryService = instantiateFactoryService;
        }

        public void Show(CharacterAction action, string controlButton, ControlDeviceType deviceType)
        {
            if (_hints.ContainsKey(action) == false)
            {
                var hint = _inputHintConfig.GetHint(deviceType, controlButton);
                var newHint = _instantiateFactoryService.Create(_hintImage, parent: _hintHolder, customName: action.ToString());
                newHint.Set(hint.ControlName, hint.Icon);
                _hints[action] = newHint;
            }
            else
            {
                _hints[action].gameObject.SetActive(true);
            }
        }

        public void Hide(CharacterAction action)
        {
            if (_hints.TryGetValue(action, out var hint))
            {
                hint.gameObject.SetActive(false);
            }
        }

        public void HideAll()
        {
            foreach (var hint in _hints.Values)
            {
                hint.gameObject.SetActive(false);
            }
            _hints.Clear();
        }

        public void Clear()
        {
            foreach (var hint in _hints.Values)
            {
                _instantiateFactoryService.Release(hint);
            }
            _hints.Clear();
        }
    }
}
    
