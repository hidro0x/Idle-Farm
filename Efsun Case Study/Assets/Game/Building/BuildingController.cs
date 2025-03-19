using System;
using System.Collections.Generic;
using UniRx;
using Zenject;
using Sirenix.OdinInspector;
using UnityEngine;

public class BuildingController 
{
    private readonly List<BuildingObject> _buildings = new List<BuildingObject>();
    
    public void AddBuilding(BuildingObject building)
    {
        if (!_buildings.Contains(building))
        {
            _buildings.Add(building);
        }
    }

    public void RemoveBuilding(BuildingObject building)
    {
        if (_buildings.Contains(building))
        {
            _buildings.Remove(building);
        }
    }

    public BuildingObject GetBuilding(BuildingObject building)
    {
        return _buildings.Contains(building) ? building : null;
    }
}