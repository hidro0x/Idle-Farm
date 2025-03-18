using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ProductionButtonsUI : MonoBehaviour
{
    [SerializeField] private Button startProductionButton, removeProductionButton;
    [SerializeField] private Image resourceIcon;
    [SerializeField] private TextMeshProUGUI resourceRequiredAmountText;
    
    public static readonly Subject<Building> OnBuildingUIRequested = new Subject<Building>();
    
    private IDisposable _resourceAddSubscription;

    private void OnEnable()
    {
        _resourceAddSubscription = OnBuildingUIRequested
            .Subscribe(Open);
    }

    private void OnDisable()
    {
        _resourceAddSubscription?.Dispose();
    }
    

    private void Open(Building building)
    {
        startProductionButton.onClick.AddListener(building.AddToProductionOrder);
        removeProductionButton.onClick.AddListener(building.RemoveFromProductionOrder);
        
        var buildingSo = building.BuildingSO;
        resourceIcon.sprite = buildingSo.ResourceSo.Icon;
        resourceRequiredAmountText.SetText($"x{buildingSo.RequiredAmount}");
    }
    
    private void Close()
    {
        startProductionButton.onClick.RemoveAllListeners();
        removeProductionButton.onClick.RemoveAllListeners();
    }
}
