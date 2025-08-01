using Service;
using UnityEngine.InputSystem;
using Zenject;

public class ToolsInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);

        Container.BindInterfacesAndSelfTo<ScreenFadeController>().AsSingle();
        Container.BindInterfacesAndSelfTo<ScreenFadeView>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<MessageBoxController>().AsSingle();
        Container.BindInterfacesAndSelfTo<MessageBoxView>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<PlayerInput>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<TooltipeView>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<PopupView>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<HintView>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<DialogueView>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<FpsCounter>().FromComponentInHierarchy().AsSingle();
    }
}
