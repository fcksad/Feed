using Zenject;


public class CharacterCommandController : ICharacterCommandController, ITickable
{
    private ICharacterCommand _currentCommand;
    private ICharacterCommand _deferredCommand;

    public void SetCommand(ICharacterCommand command)
    {
        if (_currentCommand != null && !_currentCommand.IsFinished)
        {
            _deferredCommand = command;
            return;
        }

        _currentCommand = command;
        _currentCommand.Execute();
    }

    public void Tick()
    {
        if (_currentCommand != null && _currentCommand.IsFinished)
        {
            _currentCommand = null;

            if (_deferredCommand != null)
            {
                SetCommand(_deferredCommand);
                _deferredCommand = null;
            }
        }
    }

    public bool IsBusy => _currentCommand != null && !_currentCommand.IsFinished;
}
