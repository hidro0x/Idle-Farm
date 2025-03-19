using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

public class Building : SerializedMonoBehaviour, IClickableObject
{
    [field:SerializeField] public BuildingSO BuildingSO { get; private set; }
    public BuildingData Data {get; private set; }
    public InfoSliderUI InfoUI {get; private set; }

    [Inject]
    public void Init(BuildingController controller, InfoSliderUIFactory infoSliderUIFactory)
    {
        controller.AddBuilding(this);

        Instantiate(BuildingSO.Prefab, transform.GetChild(0));
        Data = new BuildingData();
        InfoUI = infoSliderUIFactory.Create();
        InfoUI.Init(this);
    }

    public void Tick(float tickValue)
    {
        if(Data.IsProductionFinished(tickValue))ResourceController.OnResourceAddRequested.OnNext((BuildingSO.ResourceSo, BuildingSO.OutputAmount));
    }

    public void AddToProductionOrder()
    {
        if(Data.CurrentCapacity.Value + 1 > BuildingSO.Capacity) return;
        Data.AddToCapacity(1);
    }
    
    public void RemoveFromProductionOrder()
    {
        if(Data.CurrentCapacity.Value - 1 < 0) return;
        Data.RemoveFromCapacity(1);
    }

    public void OnClick() => ProductionButtonsUI.OnBuildingUIRequested.OnNext(this);
}
