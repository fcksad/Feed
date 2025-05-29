using Zenject;

public class ScreenFadeController : IInitializable
{

    private ScreenFadeView _view;
    private ISceneService _sceneService;

    [Inject]
    public void Construct(ISceneService sceneService, ScreenFadeView screenFadeView)
    {
        _sceneService = sceneService;
        _view = screenFadeView;
    }

    public void Initialize()
    {
        BindEvents();
    }

    private void BindEvents()
    {
        _sceneService.OnSceneLoadEvent += async () => await _view.FadeIn();
        _sceneService.OnSceneUnloadEvent += async () => await _view.FadeOut();
    }
}
