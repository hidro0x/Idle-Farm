using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

[System.Serializable]
public class Building
{
    private BuildingSO _info;

    private ReactiveProperty<int> _currentOrderAmount = new ReactiveProperty<int>();
    public ReactiveProperty<float> TimeLeft { get; } = new ReactiveProperty<float>();
    public ReactiveProperty<int> CurrentResourceAmount { get; } = new ReactiveProperty<int>();
    public ReactiveProperty<int> CurrentTotalCapacity { get; } = new ReactiveProperty<int>(0);

    public int ID => _info.Id;
    public GameObject Prefab => _info.BuildingPrefab;
    public float ProductionTime => _info.BaseProductionTime;
    public ResourceSO InputResource => _info.InputResource;
    public ResourceSO OutputResource => _info.OutputResource;
    public int OutputAmount => _info.BaseProductionOutputAmount;
    public int InputAmount => _info.BaseProductionInputAmount;
    public int MaxCapacity => _info.BaseCapacity;

    public bool CanAddOrder(int ownedResourceAmount) => CurrentTotalCapacity.Value + InputAmount <= MaxCapacity &&
                                                        ownedResourceAmount >= InputAmount;

    public bool CanRemoveOrder => _currentOrderAmount.Value > 0;
    public bool IsCapacityFull => CurrentResourceAmount.Value == MaxCapacity;

    private void ResetTime() => TimeLeft.Value = ProductionTime;

    public Building(BuildingSO buildingSo)
    {
        _info = buildingSo;
        TimeLeft.Value = ProductionTime;

        _currentOrderAmount
            .CombineLatest(CurrentResourceAmount, (orders, resources) => orders + resources)
            .Subscribe(total => CurrentTotalCapacity.Value = total);
    }

    public void AddOrder()
    {
        _currentOrderAmount.Value++;
        ResourceController.RequestRemoveResource(InputResource, InputAmount);
    }

    public void RemoveOrder()
    {
        _currentOrderAmount.Value--;
        ResourceController.RequestAddResource(InputResource, InputAmount);
        ResetTime();
    }

    public void Tick(float tickValue)
    {
        if (!_info.IsGenerator && _currentOrderAmount.Value == 0) return;
        if (IsCapacityFull) return;

        TimeLeft.Value -= tickValue;

        if (TimeLeft.Value <= 0)
        {
            FinishProduction();
        }
    }

    private void FinishProduction()
    {
        if (!_info.IsGenerator) _currentOrderAmount.Value--;
        CurrentResourceAmount.Value += OutputAmount;

        StartProduction();
    }

    public void StartProduction()
    {
        if (!_info.IsGenerator && _currentOrderAmount.Value == 0) return;

        ResetTime();
    }

    public void CollectResource()
    {
        if (CurrentResourceAmount.Value > 0)
        {
            int amount = CurrentResourceAmount.Value;
            CurrentResourceAmount.Value = 0;
            ResourceController.RequestAddResource(OutputResource, amount);
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

    public void SetData(BuildingData data)
    {
        _currentOrderAmount.Value = data.CurrentOrderAmount;
        TimeLeft.Value = data.TimeLeft;
        CurrentResourceAmount.Value = data.CurrentResourceAmount;
    }
    
    public BuildingData GetData()
    {
        var newData = new BuildingData
        {
            CurrentOrderAmount = _currentOrderAmount.Value,
            TimeLeft = TimeLeft.Value,
            CurrentResourceAmount = CurrentResourceAmount.Value
        };

        return newData;
    }
}

[System.Serializable]
public struct BuildingData
{
    public int CurrentOrderAmount{ get; set; }
    public int CurrentResourceAmount{ get; set; }
    public float TimeLeft{ get; set; }
    public BuildingData(int currentOrderAmount, int currentResourceAmount, float timeLeft)
    {
        CurrentOrderAmount = currentOrderAmount;
        CurrentResourceAmount = currentResourceAmount;
        TimeLeft = timeLeft;
    }
    
    
}