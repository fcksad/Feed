using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SceneButtonListener : CustomButton
{
    [SerializeField] private SceneConfig _sceneToLoad;

    private ISceneService _sceneService;

    private void Awake()
    {
        Button.onClick.AddListener(LoadScene);
    }

    [Inject]
    public void Construct(ISceneService sceneService)
    {
        _sceneService = sceneService;
    }

    private void LoadScene()
    {
        _sceneService.Transition(_sceneToLoad.Scene.SceneName);
    }

    private void OnDestroy()
    {
        Button.onClick.AddListener(LoadScene);
    }
}
