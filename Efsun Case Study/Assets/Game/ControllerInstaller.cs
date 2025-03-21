using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ControllerInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<ResourceController>().AsSingle();
        Container.Bind<BuildingController>().AsSingle();
        Container.BindInterfacesAndSelfTo<TimeController>().AsSingle();
    }
}
