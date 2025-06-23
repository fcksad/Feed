using Zenject;

public class CharacterInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<CharacterInput>().AsSingle();
        Container.BindInterfacesAndSelfTo<Character>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<ScreenPointIndicator>().FromComponentInHierarchy().AsSingle();
    }
}