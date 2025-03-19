using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;


public class BuildingSO : SerializedScriptableObject
{
    [SerializeField] private int id;
    
    [PreviewField(100, ObjectFieldAlignment.Center)]
    [SerializeField] private GameObject buildingPrefab;
    [SerializeField] private ResourceSO resourceSo;
    [SerializeField] private int baseCapacity;
    [SerializeField] private float baseProductionTime;
    [SerializeField] private int productionRequirementAmount;
    [SerializeField] private int productionOutputAmount;

    public int ID => id;
    public ResourceSO Resource => resourceSo;
    public int Capacity => baseCapacity;
    public float ProductionTime => baseProductionTime;
    public int RequiredAmount => productionRequirementAmount;
    public int OutputAmount => productionOutputAmount;
    public GameObject Prefab => buildingPrefab;


}

[System.Serializable]
public class BuildingStats
{
    public ReactiveProperty<int> CurrentCapacity { get; } = new ReactiveProperty<int>();
    public ReactiveProperty<float> TimeLeft { get; } = new ReactiveProperty<float>();
    public ReactiveProperty<int> CurrentResourceAmount { get; } = new ReactiveProperty<int>();

    private int _productionTime;
    
    public BuildingStats(int currentCapacity= 0, float timeLeft = 0)
    {
        CurrentCapacity.Value = currentCapacity;
        TimeLeft.Value = timeLeft;
    }

    public void SetTime(int productionTime) => _productionTime = productionTime;
    public void AddToCapacity(int amount) => CurrentCapacity.Value += amount;
    public void RemoveFromCapacity(int amount) => CurrentCapacity.Value -= amount;

    public int CollectResource()
    {
        var amount = CurrentCapacity.Value;
        CurrentCapacity.Value = 0;
        return amount;
    }

    public bool IsProductionFinished(float tickValue)
    {
        if (CurrentCapacity.Value <= 0) return false;
        TimeLeft.Value -= tickValue;
        if (TimeLeft.Value >= 0) return false;
        TimeLeft.Value = 0;
        CurrentCapacity.Value--;
        CurrentResourceAmount.Value++;
        return true;

    }
}
