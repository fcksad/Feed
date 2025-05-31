using System;

namespace Service
{
    public interface IInputService
    {
        void AddActionListener(CharacterAction action, Action onStarted = null, Action onPerformed = null, Action onCanceled = null);
        void RemoveActionListener(CharacterAction action, Action onStarted = null, Action onPerformed = null, Action onCanceled = null);
        bool IsPressed(CharacterAction action);
        void ChangeInputMap(InputMapType type);
        string GetActionKey(CharacterAction action);
    }

}
