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

    public GameObject Prefab => _info.BuildingPrefab;
    public float ProductionTime => _info.BaseProductionTime;
    public ResourceSO InputResource => _info.InputResource;
    public ResourceSO OutputResource => _info.OutputResource;
    public int OutputAmount => _info.BaseProductionOutputAmount;
    public int InputAmount => _info.BaseProductionInputAmount;
    public int MaxCapacity => _info.BaseCapacity;

    public bool CanAddOrder => CurrentOrderCapacity.Value < MaxCapacity;
    public bool CanRemoveOrder => CurrentOrderCapacity.Value > 0;
    
    public bool HaveEnoughSpace()
    {
        return (CurrentResourceAmount.Value + OutputAmount) <= MaxCapacity;
    }

    public Building(BuildingSO buildingSo)
    {
        _info = buildingSo;
        TimeLeft.Value = ProductionTime;
    }

    public void AddOrder()
    {
        if (CanAddOrder) 
            CurrentOrderCapacity.Value++;
    } 

    public void RemoveOrder()
    {
        if (CanRemoveOrder) 
            CurrentOrderCapacity.Value--;
    }
    
    public void Tick(float tickValue)
    {
        if (!_info.IsGenerator && CurrentOrderCapacity.Value == 0) return;

        TimeLeft.Value -= tickValue;

        if (TimeLeft.Value <= 0)
        {
            FinishProduction();
        }
    }
    
    private void FinishProduction()
    {
        if (!HaveEnoughSpace()) return;

        CurrentResourceAmount.Value += OutputAmount;
        TimeLeft.Value = 0;
        
        if (_info.IsGenerator || CurrentOrderCapacity.Value > 0)
        {
            StartProduction(); 
        }
    }
    
    public void StartProduction()
    {
        if (!_info.IsGenerator && CurrentOrderCapacity.Value <= 0) return;
        if (!HaveEnoughSpace()) return;  

        TimeLeft.Value = ProductionTime;
        if (!_info.IsGenerator) 
            RemoveOrder();
    }
    
    public void CollectResource()
    {
        if (CurrentResourceAmount.Value > 0)
        {
            int amount = CurrentResourceAmount.Value;
            CurrentResourceAmount.Value = 0;
            ResourceController.OnResourceAddRequested.OnNext((OutputResource, amount));

            if (_info.IsGenerator)
            {
                StartProduction();
            }
        }
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
