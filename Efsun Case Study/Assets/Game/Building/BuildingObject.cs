using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using UniRx;

public class BuildingObject : SerializedMonoBehaviour, IClickableObject
{
    [field: SerializeField] private BuildingSO attachedBuilding;
    public Building Building {get; private set; }
    [field: SerializeField]public InfoSliderUI InfoUI {get; private set; }
    
    [Inject]
    public void Init(BuildingController buildingController,TimeController timeController)
    {
        Building = new Building(attachedBuilding);
        buildingController.AddBuilding(this);
        
        Instantiate(Building.Prefab, transform.GetChild(0));
        
        InfoUI.Init(Building);
        
        timeController.OnTick
            //Linq daha maliyetli olabileceginden gerek duymadim fakaStatusunun scaleine gore degisir
            //.Where(_ => Data.CurrentCapacity.Value > 0)
            .Subscribe(Building.Tick) 
            .AddTo(this);
    }
    

    public void OnClicked() => Building.Interact(this);

}
