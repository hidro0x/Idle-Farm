using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public TimeController timeManager; 

    public override void InstallBindings()
    {
        Container.Bind<TimeController>().FromInstance(timeManager).AsSingle();
    }
}
