using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class BuildingSO : SerializedScriptableObject
{
    [SerializeField] private int id;

    [SerializeField] private Resource resource;
    [SerializeField] private int baseCapacity;
    [SerializeField] private float baseProductionTime;
    [SerializeField] private int productionRequirementAmount;
    [SerializeField] private int productionOutputAmount;

    public Resource Resource => resource;
    public int Capacity => baseCapacity;
    public float ProductionTime => baseProductionTime;
    public int RequiredAmount => productionRequirementAmount;
    public int OutputAmount => productionOutputAmount;
    
}

[System.Serializable]
public class BuildingData
{
    public int CurrentCapacity { get; private set; }
    public float TimeLeft { get; private set; }

    public BuildingData(int currentCapacity, float timeLeft)
    {
        CurrentCapacity = currentCapacity;
        TimeLeft = timeLeft;
    }

    public void AddToCapacity(int amount) => CurrentCapacity += amount;
    public void RemoveFromCapacity(int amount) => CurrentCapacity -= amount;

    public bool IsProductionFinished(float tickValue)
    {
        if (CurrentCapacity <= 0) return false;
        TimeLeft -= tickValue;
        if (TimeLeft >= 0) return false;
        CurrentCapacity--;
        return true;

    }
}
