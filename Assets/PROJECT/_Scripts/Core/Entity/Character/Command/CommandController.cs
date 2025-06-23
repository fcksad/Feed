public class CommandController : ICommandController
{
    private ICommand _currentCommand;
    private ICommand _deferredCommand;

    public void SetCommand(ICommand command)
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
}
