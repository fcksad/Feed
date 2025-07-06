using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace Service
{
    public interface IControlsService
    {
       // List<InputAction> GetAllActions();
        void Rebinding(InputAction action, Guid bindingId);
        void Binding(string name, int bindingIndex, Action value);
        InputActionMap GetFirstActionMap();
    }
}
