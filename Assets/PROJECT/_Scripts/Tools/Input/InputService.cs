using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Service
{
    public class InputService : IInputService, IInitializable, IDisposable
    {
        private PlayerInput _playerInput;
        private List<InputActionConfigBase> _inputActions = new List<InputActionConfigBase>();
        private Dictionary<CharacterAction, InputActionConfigBase> _actionDictionary = new Dictionary<CharacterAction, InputActionConfigBase>();

        [Inject]
        public InputService(PlayerInput playerInput)
        {
            _playerInput = playerInput;
        }

        public void Initialize()
        {
            _inputActions = ResourceLoader.GetAll<InputActionConfigBase>();

            foreach (var action in _inputActions)
            {
                if (action is IInputAction inputAction)
                {
                    inputAction.Initialize();
                }

                if (!_actionDictionary.ContainsKey(action.Action))
                {
                    _actionDictionary[action.Action] = action;
                }
            }
        }

        public void AddActionListener(CharacterAction action, Action onStarted = null, Action onPerformed = null, Action onCanceled = null)
        {
            if (_actionDictionary.TryGetValue(action, out var config))
            {
                if (onStarted != null) config.OnStartedEvent += onStarted;
                if (onPerformed != null) config.OnPerformedEvent += onPerformed;
                if (onCanceled != null) config.OnCanceledEvent += onCanceled;
            }
        }

        public void RemoveActionListener(CharacterAction action, Action onStarted = null, Action onPerformed = null, Action onCanceled = null)
        {
            if (_actionDictionary.TryGetValue(action, out var config))
            {
                if (onStarted != null) config.OnStartedEvent -= onStarted;
                if (onPerformed != null) config.OnPerformedEvent -= onPerformed;
                if (onCanceled != null) config.OnCanceledEvent -= onCanceled;
            }
        }

        public bool IsPressed(CharacterAction action)
        {
            return _actionDictionary.TryGetValue(action, out var config) && config.IsPressed();
        }

        public void ChangeInputMap(InputMapType type)
        {
            foreach (var map in _playerInput.actions.actionMaps)
            {
                map.Disable();
            }

            var targetMap = _playerInput.actions.FindActionMap(type.ToString(), true);
            if (targetMap != null)
            {
                targetMap.Enable();
                Debug.Log($"Activated Input Map: {type}");
            }
            else
            {
                Debug.LogWarning($"InputActionMap with name {type} not found.");
            }
        }

        public void Dispose()
        {
            foreach (var action in _inputActions)
            {
                if (action is IInputAction inputAction)
                {
                    inputAction.Cleanup();
                }
            }

            _actionDictionary.Clear();
        }
    }

}
