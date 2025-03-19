using System;
using System.Collections.Generic;
using UniRx;
using Zenject;
using Sirenix.OdinInspector;
using UnityEngine;

public class BuildingController 
{
    private readonly List<Building> _buildings = new List<Building>();
    
    public void AddBuilding(Building building)
    {
        if (!_buildings.Contains(building))
        {
            _buildings.Add(building);
        }
    }

    public void RemoveBuilding(Building building)
    {
        if (_buildings.Contains(building))
        {
            _buildings.Remove(building);
        }
    }

    public Building GetBuilding(Building building)
    {
        return _buildings.Contains(building) ? building : null;
    }
}