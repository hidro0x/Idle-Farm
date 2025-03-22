using System;
using System.Collections.Generic;
using UniRx;
using Zenject;
using Sirenix.OdinInspector;
using UnityEngine;

public class BuildingController : ISaveable
{

    private readonly Dictionary<int, Building> _buildings = new Dictionary<int, Building>();

    [Inject]
    public void Construct(DataService dataService)
    {
        dataService.Register(this);
    }
    public void AddBuilding(Building building)
    {
        if (!_buildings.ContainsKey(building.ID))
        {
            _buildings.Add(building.ID, building);
        }
    }

    public void RemoveBuilding(Building building)
    {
        if (_buildings.ContainsKey(building.ID))
        {
            _buildings.Remove(building.ID);
        }
    }
    

    public void LoadData(Data data)
    {
        var totalTimePassed = DateTime.UtcNow.Subtract(data.LastSavedTime).TotalSeconds;
        foreach (var buildingData in data.BuildingDatas)    
        {
            if (_buildings.ContainsKey(buildingData.Key))
            {
                _buildings[buildingData.Key].SetData(buildingData.Value);
                
            }
        }

        foreach (var building in _buildings)
        {
            building.Value.SkipTime(Convert.ToInt32(totalTimePassed));
        }
    }

    public void SaveData(Data data)
    {
        foreach (var building in _buildings)    
        {
            if (data.BuildingDatas.ContainsKey(building.Key))
            {
                data.BuildingDatas[building.Key] = building.Value.GetData();
            }
            else
            {
                data.BuildingDatas.Add(building.Key, building.Value.GetData());
            }
        }
    }
}