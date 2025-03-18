using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class BuildingUI : MonoBehaviour
{
    [SerializeField] private Button startProductionButton, removeProductionButton;
    [SerializeField] private InfoSliderUI infoSliderUI;
    private RectTransform _rect;
    private Camera _camera;
    
    public static readonly Subject<(Building building, Vector3 position)> OnBuildingUIRequested = new Subject<(Building, Vector3)>();
    
    private IDisposable _resourceAddSubscription;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _camera = Camera.main;
    }

    private void OnEnable()
    {
        _resourceAddSubscription = OnBuildingUIRequested
            .Subscribe(tuple => Open(tuple.building, tuple.position));
    }

    private void OnDisable()
    {
        _resourceAddSubscription?.Dispose();
    }
    

    private void Open(Building building, Vector3 position)
    {
        _rect.position = _camera.WorldToScreenPoint(position);
        startProductionButton.onClick.AddListener(building.AddToProductionOrder);
        removeProductionButton.onClick.AddListener(building.RemoveFromProductionOrder);
        infoSliderUI.Init(building);
    }
    
    private void Close()
    {
        _rect.position = new Vector3(99999,99999,99999);
        startProductionButton.onClick.RemoveAllListeners();
        removeProductionButton.onClick.RemoveAllListeners();
        infoSliderUI.Close();
    }
}
