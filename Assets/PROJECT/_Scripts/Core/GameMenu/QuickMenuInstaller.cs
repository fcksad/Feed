using Zenject;

public class QuickMenuInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<QuickMenuController>().AsSingle();
        Container.BindInterfacesAndSelfTo<QuickMenuView>().FromComponentInHierarchy().AsSingle();
    }
}
