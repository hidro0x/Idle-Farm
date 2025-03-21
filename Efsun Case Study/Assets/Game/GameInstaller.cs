using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private GameSettings gameSettings;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<DataService>().AsSingle().NonLazy();

        Container.Bind<ISaveable>().To<ResourceController>().AsSingle();
        Container.Bind<ISaveable>().To<BuildingController>().AsSingle();
        
        Container.BindInterfacesAndSelfTo<TimeController>().AsSingle();
        Container.Bind<GameSettings>().FromScriptableObject(gameSettings).AsSingle();
    }
}