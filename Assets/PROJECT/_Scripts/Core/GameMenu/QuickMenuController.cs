using Service;
using System;
using Zenject;

public class QuickMenuController : IInitializable, IDisposable
{
    private QuickMenuView _quickMenuView;

    private IInputService _inputService;
    private CharacterInput _characterInput;

    private bool _active = false;

    [Inject]
    public QuickMenuController(QuickMenuView quickMenuView, IInputService inputService, CharacterInput characterInput)
    {
        _quickMenuView = quickMenuView;
        _inputService = inputService;
        _characterInput = characterInput;   
    }

    public void Initialize()
    {
        _inputService.AddActionListener(CharacterAction.Menu, onStarted: ToggleMenu);
        _quickMenuView.ToggleButton.Button.onClick.AddListener(ToggleMenu);

    }

    public void Dispose()
    {
        _inputService.RemoveActionListener(CharacterAction.Menu, onStarted: ToggleMenu);
        _quickMenuView.ToggleButton.Button.onClick.RemoveListener(ToggleMenu);
    }

    public void ToggleMenu()
    {
        _active = !_active;
        _quickMenuView.Toggle(_active);
        _characterInput.Lock(_active);
    }
}
