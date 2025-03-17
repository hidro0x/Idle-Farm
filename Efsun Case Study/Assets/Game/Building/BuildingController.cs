using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using UniRx;

public class BuildingController : SerializedMonoBehaviour
{
    [Inject]private TimeController _timeController;
    private List<Building> _buildings;

    private void Start()
    {
        foreach (var building in _buildings)
        {
            _timeController.OnTick
                .Subscribe(tickValue => building.Tick(tickValue))
                .AddTo(building);
        }
        
    }
    
}
