using Zenject;

public class CharacterInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<CharacterInput>().AsSingle();
        Container.BindInterfacesAndSelfTo<CharacterController>().AsSingle();
        Container.BindInterfacesAndSelfTo<CommandController>().AsSingle();
        Container.BindInterfacesAndSelfTo<Character>().FromComponentInHierarchy().AsSingle();
    }
}