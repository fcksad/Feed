using UnityEngine;
using Zenject;

public class EntityInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<EntityTickService>().AsSingle();

    }
}
