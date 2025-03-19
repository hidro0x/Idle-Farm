using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    private TimeController _timeController;
    private BuildingController _buildingController;

    [Inject]
    private void Construct(TimeController timeController, BuildingController buildingController)
    {
        _timeController = timeController;
        _buildingController = buildingController;
    }
}
