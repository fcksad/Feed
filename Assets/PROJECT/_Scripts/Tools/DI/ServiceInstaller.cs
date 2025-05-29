using Localization;
using Service;
using Zenject;

public class ServiceInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<SceneService>().AsSingle();
        Container.BindInterfacesAndSelfTo<SaveService>().AsSingle();
        Container.BindInterfacesAndSelfTo<InputService>().AsSingle();
        Container.BindInterfacesAndSelfTo<AudioService>().AsSingle();
        Container.BindInterfacesAndSelfTo<GraphicsService>().AsSingle();
        Container.BindInterfacesAndSelfTo<LocalizationService>().AsSingle();
        Container.BindInterfacesAndSelfTo<ControlsService>().AsSingle();
        Container.BindInterfacesAndSelfTo<TooltipService>().AsSingle();
        Container.BindInterfacesAndSelfTo<PopupService>().AsSingle();
        Container.BindInterfacesAndSelfTo<InstantiateFactoryService>().AsSingle();
        Container.BindInterfacesAndSelfTo<ParticlService>().AsSingle();
        Container.BindInterfacesAndSelfTo<PoolService>().AsSingle();
    }
}
