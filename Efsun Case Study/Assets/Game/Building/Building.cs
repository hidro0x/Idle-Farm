using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

[System.Serializable]
public class Building
{
    private BuildingSO _info;
    
    public ReactiveProperty<int> CurrentOrderCapacity { get; } = new ReactiveProperty<int>();
    public ReactiveProperty<float> TimeLeft { get; } = new ReactiveProperty<float>();
    public ReactiveProperty<int> CurrentResourceAmount { get; } = new ReactiveProperty<int>();

    //Ilerleyen asamalarda 
    //public float ProductionTime => _info.BaseProductionTime * (level*timeMultipler);
    //etc. yapilabilir
    public GameObject Prefab => _info.BuildingPrefab;
    public float ProductionTime => _info.BaseProductionTime;
    public ResourceSO InputResource => _info.InputResource;
    public ResourceSO OutputResource => _info.OutputResource;
    public int OutputAmount => _info.BaseProductionOutputAmount;
    public int InputAmount => _info.BaseProductionInputAmount;
    public int MaxCapacity => _info.BaseCapacity;

    public bool CanAddOrder => CurrentOrderCapacity.Value + 1 <= MaxCapacity;
    public bool CanRemoveOrder => CurrentOrderCapacity.Value - 1 >= 0;

    public Building(BuildingSO buildingSo)
    {
        _info = buildingSo;
        TimeLeft.Value = ProductionTime;

    }

    public void AddOrder()
    {
        if (CanAddOrder) CurrentOrderCapacity.Value++;

    } 

    public void RemoveOrder()
    {
        if(CanRemoveOrder)CurrentOrderCapacity.Value--;
    }
    
    public void Tick(float tickValue)
    {
        if(!_info.IsGenerator && CurrentOrderCapacity.Value == 0) return;
        if (TimeLeft.Value - tickValue <= 0)
        {
            FinishProduction();
        } else TimeLeft.Value -= tickValue;
        
    }
    
    
    private void FinishProduction()
    {
        CurrentResourceAmount.Value += OutputAmount;
        TimeLeft.Value = 0;
        StartProduction();
    }
    
    
    public void StartProduction()
    {
        if (!_info.IsGenerator && CurrentOrderCapacity.Value <= 0) return;
        TimeLeft.Value = ProductionTime;
        RemoveOrder();

    }
    
    public void CollectResource()
    {
        var amount = CurrentResourceAmount.Value;
        CurrentResourceAmount.Value = 0;
        ResourceController.OnResourceAddRequested.OnNext((OutputResource, amount));
    }

    

    public void Interact(BuildingObject buildingObject)
    {
        if (!_info.IsGenerator) 
        {
            ProductionButtonsUI.OnBuildingUIRequested.OnNext(buildingObject);
            return;
        }
        CollectResource();
        
        
    }
}
