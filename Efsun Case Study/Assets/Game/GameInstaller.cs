using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField]private GameSettings gameSettings;
    
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<TimeController>().AsSingle();
        
        Container.Bind<BuildingController>().AsSingle();
        Container.Bind<ResourceController>().AsSingle();
        
        
        Container.Bind<GameSettings>().FromScriptableObject(gameSettings).AsSingle();
    }
}
