using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameInstaller : MonoInstaller
{
    //ScriptableObjects
    [SerializeField]private GameSettings gameSettings;
    
    //Mono's
    [SerializeField]private HorizontalLayoutGroup resourceBarsTransform;
    
    //Controller's
    private TimeController timeController;
    private BuildingController buildingController;
    
    //Prefabs
    [SerializeField]private InfoSliderUI infoSliderUI;
    [SerializeField]private ResourceBarUI resourceBarUI;
    public override void InstallBindings()
    {
        Container.Bind<TimeController>().AsSingle();
        Container.Bind<BuildingController>().AsSingle();
        Container.Bind<ResourceController>().AsSingle();

        Container.Bind<HorizontalLayoutGroup>().FromInstance(resourceBarsTransform).AsSingle();
        
        Container.BindFactory<InfoSliderUI, InfoSliderUIFactory>().FromComponentInNewPrefab(infoSliderUI).AsTransient();
        Container.BindFactory<ResourceBarUI, ResourceBarUIFactory>().FromComponentInNewPrefab(resourceBarUI).AsTransient();
        
        Container.Bind<GameSettings>().FromScriptableObject(gameSettings).AsSingle();
    }
}
