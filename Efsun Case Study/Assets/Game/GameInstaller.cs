using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameInstaller : MonoInstaller
{
    //ScriptableObjects
    [SerializeField]private GameSettings gameSettings;
    //Controller's
    private TimeController timeController;
    private BuildingController buildingController;
    
    public override void InstallBindings()
    {
        Container.Bind<TimeController>().AsSingle();
        Container.Bind<BuildingController>().AsSingle();
        Container.Bind<ResourceController>().AsSingle();
        
        
        Container.Bind<GameSettings>().FromScriptableObject(gameSettings).AsSingle();
    }
}
