using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class BuildingSO : SerializedScriptableObject
{
    [SerializeField] private int id;
    
    [SerializeField] private int baseCapacity;
    [SerializeField] private float baseProductionTime;
    [SerializeField] private int productionRequirementAmount;
    [SerializeField] private int productionOutputAmount;

    public int Capacity => baseCapacity;
    public float ProductionTime => baseProductionTime;
    public int RequiredAmount => productionRequirementAmount;
    public int OutputAmount => productionOutputAmount;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]
public class BuildingData
{
    public int CurrentCapacity { get; private set; }
    public float TimeLeft { get; set; }
    public Action OnProductionFinished;
    
    public BuildingData(int currentCapacity, float timeLeft)
    {
        CurrentCapacity = currentCapacity;
        TimeLeft = timeLeft;
    }

    public void Tick(float tickValue)
    {
        if (CurrentCapacity < 0) return;
        TimeLeft -= tickValue;
        if (TimeLeft < 0)
        {
            CurrentCapacity--;
            
        }
    }
}
