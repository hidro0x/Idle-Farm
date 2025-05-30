using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
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

    private void AddResource(int amount)
    {
        CurrentResourceAmount.Value = Mathf.Min(CurrentResourceAmount.Value + amount, MaxCapacity);
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
        ResetTime();
    }
    

    public void CollectResource(Vector3 pos)
    {
        if (CurrentResourceAmount.Value > 0)
        {
            int amount = CurrentResourceAmount.Value;
            CurrentResourceAmount.Value = 0;
            ResourceController.RequestAddResource(OutputResource, amount);
            ResourceCollectAnimation.OnResourceCollected?.Invoke(pos, OutputResource, amount);
            if (_info.IsGenerator)
            {
                ResetTime();
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

        CollectResource(buildingObject.transform.position);
    }
    
    public void SkipTime(int skipAmount)
    {
        float remainingTime = skipAmount;
        
        if (_info.IsGenerator)
        {
            int producedCount = Mathf.FloorToInt(remainingTime / ProductionTime);
            int maxProducible = (MaxCapacity - CurrentResourceAmount.Value) / OutputAmount;
            int actualProduced = Mathf.Min(producedCount, maxProducible);

            AddResource(actualProduced * OutputAmount);

            TimeLeft.Value = (CurrentResourceAmount.Value >= MaxCapacity) ? ProductionTime : ProductionTime - (remainingTime % ProductionTime);
            return;
        }
        
        if (_currentOrderAmount.Value <= 0) return;
        
        if (remainingTime >= TimeLeft.Value)
        {
            remainingTime -= TimeLeft.Value;
            AddResource(OutputAmount);
            _currentOrderAmount.Value--;
        }
        else
        {
            TimeLeft.Value -= remainingTime;
            return;
        }
        
        int remainingOrders = _currentOrderAmount.Value;
        int producibleOrders = Mathf.FloorToInt(remainingTime / ProductionTime);
        int actualCompleted = Mathf.Min(producibleOrders, remainingOrders);

        AddResource(actualCompleted * OutputAmount);
        _currentOrderAmount.Value -= actualCompleted;
        remainingTime -= actualCompleted * ProductionTime;
        
        TimeLeft.Value = (_currentOrderAmount.Value > 0) ? ProductionTime - remainingTime : ProductionTime;
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
            timeLeft = TimeLeft.Value,
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
    public float timeLeft;
    public BuildingData(int currentOrderAmount, int currentResourceAmount, float timeLeft)
    {
        this.currentOrderAmount = currentOrderAmount;
        this.currentResourceAmount = currentResourceAmount;
        this.timeLeft = timeLeft;
    }
    
    
}