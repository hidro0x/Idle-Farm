using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

public class BuildingSO : SerializedScriptableObject
{
    [field:Title("General Information")]
    [field: MinValue(1)] 
    [field: SerializeField] public int Id { get; private set; }
    [field: Tooltip("If its building that not require any input resource.")]
    [field: OnValueChanged("SetAsGenerator")]
    [field: SerializeField] public bool IsGenerator { get; private set; }

    [field:Title("Building Prefab")]
    [field: PreviewField(100, ObjectFieldAlignment.Center)]
    [field: Required("Can not be empty.")]
    [field: SerializeField] public GameObject BuildingPrefab { get; private set; }

    [field:Title("Resources")]
    [field: Required("Can not be empty.")]
    [field: SerializeField] public ResourceSO OutputResource { get; private set; }
    [field: HideIf("IsGenerator")]
    [field: Required("Can not be empty.")]
    [field: SerializeField] public ResourceSO InputResource { get; private set; }

    [field:Title("Production Details")]
    [field: MinValue(1)]
    [field: SerializeField] public int BaseCapacity { get; private set; }
    [field: MinValue(1)] 
    [field: SerializeField] public float BaseProductionTime { get; private set; }
    [field: HideIf("IsGenerator")]
    [field: MinValue(1)] 
    [field: SerializeField] public int BaseProductionInputAmount { get; private set; }
    [field: MinValue(1)] 
    [field: ValidateInput("IsOverCapacity", "Can't be bigger number than maximum capacity.")]
    [field: SerializeField] public int BaseProductionOutputAmount { get; private set; }
    
    private bool IsOverCapacity(int num)
    {
        return BaseProductionOutputAmount <= BaseCapacity;
    }

    private void SetAsGenerator()
    {
        BaseProductionInputAmount = 0;
        InputResource = null;
    }
}


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

    public Building(BuildingSO buildingSo)
    {
        _info = buildingSo;
    }
    private void AddToCapacity(int amount) => CurrentOrderCapacity.Value += amount;
    private void RemoveFromCapacity(int amount) => CurrentOrderCapacity.Value -= amount;
    public void Tick(float tickValue)
    {
        if (TimeLeft.Value - tickValue < 0)
        {
            FinishProduction();
        }
        else TimeLeft.Value -= tickValue;
    }
    
    public void AddToProductionOrder()
    {
        if(CurrentOrderCapacity.Value + 1 > MaxCapacity) return;
        AddToCapacity(1);
    }
    
    public void RemoveFromProductionOrder()
    {
        if(CurrentOrderCapacity.Value - 1 < 0) return;
        RemoveFromCapacity(1);
    }
    public int CollectResource()
    {
        var amount = CurrentOrderCapacity.Value;
        CurrentOrderCapacity.Value = 0;
        return amount;
    }

    private void FinishProduction()
    {
        CurrentOrderCapacity.Value--;
        CurrentResourceAmount.Value += OutputAmount;
        ResourceController.OnResourceAddRequested.OnNext((OutputResource, OutputAmount));
        StartProduction();
    }
    
    
    public void StartProduction()
    {
        if(CurrentOrderCapacity.Value <= 0) return;
        TimeLeft.Value = ProductionTime;
    }

    public void Interact(BuildingObject buildingObject)
    {
        if (_info.IsGenerator)
        {
            ResourceController.OnResourceAddRequested.OnNext((InputResource, CollectResource()));
            return;
        }
        ProductionButtonsUI.OnBuildingUIRequested.OnNext(buildingObject);
    }
}
