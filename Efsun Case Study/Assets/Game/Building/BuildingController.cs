using System;
using System.Collections.Generic;
using UniRx;
using Zenject;
using Sirenix.OdinInspector;
using UnityEngine;

public class BuildingController : SerializedMonoBehaviour
{
    private readonly List<Building> _buildings = new List<Building>();
    private TimeController _timeController;

    [Inject]
    public void Construct(TimeController timeController)
    {
        _timeController = timeController;
    }

    private void Start()
    {
        _timeController.OnTick
            .Subscribe(TickBuildings)
            .AddTo(this);
    }

    private void TickBuildings(float tickValue)
    {
        foreach (var building in _buildings)
        {
            building.Tick(tickValue);
        }
    }

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