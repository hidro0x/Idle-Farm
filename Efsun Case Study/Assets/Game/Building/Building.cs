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
    public int ProductionTime => _info.BaseProductionTime;
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
        ResetTime();

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
    
    public void SkipTime(int skipAmount)
    {
        var totalCurrentTimeLeft = (int)TimeLeft.Value + (_currentOrderAmount.Value * ProductionTime);
        if (skipAmount >= totalCurrentTimeLeft)
        {
            CurrentResourceAmount.Value += _currentOrderAmount.Value;
            _currentOrderAmount.Value = 0;
            TimeLeft.Value = 0;
            return;
        }
        
        int a = (skipAmount / ProductionTime); //Kac adet order tamamladigi
        int b = skipAmount % ProductionTime; //Geriye kalan zamanin kac oldugu
            
        CurrentResourceAmount.Value += a;
        _currentOrderAmount.Value -= a;
        TimeLeft.Value = b;
    }
    
    public void SetData(BuildingData data)
    {
        _currentOrderAmount.Value = data.currentOrderAmount;
        TimeLeft.Value = data.timeLeft;
        CurrentResourceAmount.Value = data.currentResourceAmount;
    }
    
    public BuildingData GetData()
    {
        var newData = new BuildingData
        {
            currentOrderAmount = _currentOrderAmount.Value,
            timeLeft = (int)TimeLeft.Value,
            currentResourceAmount = CurrentResourceAmount.Value
        };

        return newData;
    }
}

[System.Serializable]
public struct BuildingData
{
    public int currentOrderAmount;
    public int currentResourceAmount;
    public int timeLeft;
    public BuildingData(int currentOrderAmount, int currentResourceAmount, int timeLeft)
    {
        this.currentOrderAmount = currentOrderAmount;
        this.currentResourceAmount = currentResourceAmount;
        this.timeLeft = timeLeft;
    }
    
    
}