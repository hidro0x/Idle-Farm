using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Building : SerializedMonoBehaviour
{
    [SerializeField] private BuildingSO building;
    private BuildingData _data;

    public void Tick(float tickValue)
    {
        if(_data.IsProductionFinished(tickValue))ResourceController.OnResourceAddRequested.OnNext((building.Resource, building.OutputAmount));
    }

    public void AddToProductionOrder()
    {
        if(_data.CurrentCapacity + 1 > building.Capacity) return;
        _data.AddToCapacity(1);
    }
    
    public void RemoveFromProductionOrder()
    {
        if(_data.CurrentCapacity - 1 < 0) return;
        _data.RemoveFromCapacity(1);
    }

}
