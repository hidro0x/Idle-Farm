using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Building : SerializedMonoBehaviour, IClickableObject
{
    [field:SerializeField] public BuildingSO BuildingSO { get; private set; }
    public BuildingData Data {get; private set; }
    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    public void Tick(float tickValue)
    {
        if(Data.IsProductionFinished(tickValue))ResourceController.OnResourceAddRequested.OnNext((BuildingSO.Resource, BuildingSO.OutputAmount));
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

    public void OnClick() => BuildingUI.OnBuildingUIRequested.OnNext((this, _collider.bounds.center));
}
