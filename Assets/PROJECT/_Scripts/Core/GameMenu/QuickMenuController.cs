using Service;
using System;
using Zenject;

public class QuickMenuController : IInitializable, IDisposable
{
    private QuickMenuView _quickMenuView;

    private IInputService _inputService;
    private CharacterInput _characterInput;

    [Inject]
    public QuickMenuController(QuickMenuView quickMenuView, IInputService inputService, CharacterInput characterInput)
    {
        _quickMenuView = quickMenuView;
        _inputService = inputService;
        _characterInput = characterInput;   
    }

    public void Initialize()
    {
        _inputService.AddActionListener(CharacterAction.Menu, onStarted: EnableMenu);
        _quickMenuView.ToggleButton.Button.onClick.AddListener(DisableMenu);
    }

    public void Dispose()
    {
        _inputService.RemoveActionListener(CharacterAction.Menu, onStarted: EnableMenu);
        _quickMenuView.ToggleButton.Button.onClick.RemoveListener(DisableMenu);
    }
    public void EnableMenu()
    {
        _inputService.RemoveActionListener(CharacterAction.Menu, onStarted: EnableMenu);

        _quickMenuView.Toggle(true);
        _characterInput.Lock(true);
    }

    public void DisableMenu()
    {
        _quickMenuView.Toggle(false);
        _characterInput.Lock(false);

        _inputService.AddActionListener(CharacterAction.Menu, onStarted: EnableMenu);
    }

}
