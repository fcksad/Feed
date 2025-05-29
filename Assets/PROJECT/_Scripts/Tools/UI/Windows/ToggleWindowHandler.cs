using System;
using Zenject;

public class ToggleWindowHandler : IInitializable, IDisposable
{
    private readonly SignalBus _signalBus;
    private readonly ToggleWindow _toggleWindow;

    public ToggleWindowHandler(SignalBus signalBus, ToggleWindow toggleWindow)
    {
        _signalBus = signalBus;
        _toggleWindow = toggleWindow;
    }

    public void Initialize()
    {
        _signalBus.Subscribe<ToggleMiniGameWindowSignal>(OnToggleWindow);
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<ToggleMiniGameWindowSignal>(OnToggleWindow);
    }

    private void OnToggleWindow(ToggleMiniGameWindowSignal signal)
    {
        _toggleWindow.Toggle(signal.Show);
    }
}

public class ToggleMiniGameWindowSignal
{
    public bool Show;

    public ToggleMiniGameWindowSignal(bool show)
    {
        Show = show;
    }
}