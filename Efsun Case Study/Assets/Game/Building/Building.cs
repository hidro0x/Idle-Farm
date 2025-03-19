using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using UniRx;

public class Building : SerializedMonoBehaviour, IClickableObject
{
    [field: SerializeField] public BuildingSO BuildingSO { get; private set; }
    public BuildingStats Stats {get; private set; }
    public InfoSliderUI InfoUI {get; private set; }

    private IDisposable _timeSubscription;
    
    [Inject]
    public void Init(BuildingController buildingController,TimeController timeController, InfoSliderUIFactory infoSliderUIFactory)
    {
        buildingController.AddBuilding(this);
        
        _timeSubscription = timeController.OnTick
            //Linq daha maliyetli olabileceginden gerek duymadim fakaStatusunun scaleine gore degisir
            //.Where(_ => Data.CurrentCapacity.Value > 0)
            .Subscribe(Tick) 
            .AddTo(this);
        
        Instantiate(BuildingSO.Prefab, transform.GetChild(0));
        Stats = new BuildingStats();
        InfoUI = infoSliderUIFactory.Create();
        InfoUI.Init(this);
    }

    public void Tick(float tickValue)
    {
        if(Stats.IsProductionFinished(tickValue))ResourceController.OnResourceAddRequested.OnNext((BuildingSO.Resource, BuildingSO.OutputAmount));
    }

    public void AddToProductionOrder()
    {
        if(Stats.CurrentCapacity.Value + 1 > BuildingSO.Capacity) return;
        Stats.AddToCapacity(1);
    }
    
    public void RemoveFromProductionOrder()
    {
        if(Stats.CurrentCapacity.Value - 1 < 0) return;
        Stats.RemoveFromCapacity(1);
    }

    public void OnClicked()
    {
        if(BuildingSO.Capacity == 0) return;
        ProductionButtonsUI.OnBuildingUIRequested.OnNext(this);
    }
    
    private void CollectResources()
    {
        ResourceController.OnResourceAddRequested.OnNext((BuildingSO.Resource, Stats.CollectResource()));
    }
}
