using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using UniRx;

public class BuildingObject : SerializedMonoBehaviour, IClickableObject
{
    [field: SerializeField] private BuildingSO attachedBuilding;
    public Building Building {get; private set; }
    [field: SerializeField]public InfoSliderUI InfoUI {get; private set; }

    private Transform _buildingTransform;
    private Tweener _clickAnim;
    
    [Inject]
    public void Init(BuildingController buildingController,TimeController timeController)
    {
        Building = new Building(attachedBuilding);
        buildingController.AddBuilding(this);
        
        _buildingTransform = Instantiate(Building.Prefab, transform.GetChild(0)).transform;
        
        InfoUI.Init(Building);
        
        timeController.OnTick
            //Linq daha maliyetli olabileceginden gerek duymadim fakaStatusunun scaleine gore degisir
            //.Where(_ => Data.CurrentCapacity.Value > 0)
            .Subscribe(Building.Tick) 
            .AddTo(this);
    }

    private void Start()
    {
        _clickAnim = _buildingTransform.DOPunchScale(Vector3.one*0.1f, 0.3f, vibrato:0,elasticity:1).SetAutoKill(false).Pause();
    }


    public void OnClicked()
    {
        Building.Interact(this);
        _clickAnim.Restart();
    }

}
