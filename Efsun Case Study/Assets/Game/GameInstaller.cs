using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    //ScriptableObjects
    [SerializeField]private GameSettings gameSettings;
    
    //Mono's
    private TimeController timeController;
    private BuildingController buildingController;
    
    //Prefabs
    [SerializeField]private InfoSliderUI infoSliderUI;

    public override void InstallBindings()
    {
        Container.Bind<TimeController>().AsSingle();
        Container.Bind<BuildingController>().AsSingle();
        Container.BindFactory<InfoSliderUI, InfoSliderUIFactory>().FromComponentInNewPrefab(infoSliderUI).AsTransient();
        Container.Bind<GameSettings>().FromScriptableObject(gameSettings).AsSingle();
    }
}
