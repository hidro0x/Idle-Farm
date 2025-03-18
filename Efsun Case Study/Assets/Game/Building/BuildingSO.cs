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
    public ResourceSO ResourceSo => resourceSo;
    public int Capacity => baseCapacity;
    public float ProductionTime => baseProductionTime;
    public int RequiredAmount => productionRequirementAmount;
    public int OutputAmount => productionOutputAmount;
    
}

[System.Serializable]
public class BuildingData
{
    public ReactiveProperty<int> CurrentCapacity { get; } = new ReactiveProperty<int>();
    public ReactiveProperty<float> TimeLeft { get; } = new ReactiveProperty<float>();
    public ReactiveProperty<int> CurrentResourceAmount { get; } = new ReactiveProperty<int>();
    public BuildingData(int currentCapacity, float timeLeft)
    {
        CurrentCapacity.Value = currentCapacity;
        TimeLeft.Value = timeLeft;
    }

    public void AddToCapacity(int amount) => CurrentCapacity.Value += amount;
    public void RemoveFromCapacity(int amount) => CurrentCapacity.Value -= amount;

    public bool IsProductionFinished(float tickValue)
    {
        if (CurrentCapacity.Value <= 0) return false;
        TimeLeft.Value -= tickValue;
        if (TimeLeft.Value >= 0) return false;
        CurrentCapacity.Value--;
        CurrentResourceAmount.Value++;
        return true;

    }
}
