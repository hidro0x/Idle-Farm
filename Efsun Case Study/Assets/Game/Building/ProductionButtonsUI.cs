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

    private RectTransform _rect;
    private Canvas _canvas;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _canvas = GetComponent<Canvas>();
    }
    
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

        _canvas.enabled = true;
        _rect.position = building.InfoUI.Rect.position;
        
        var buildingSo = building.BuildingSO;
        resourceIcon.sprite = buildingSo.ResourceSo.Icon;
        resourceRequiredAmountText.SetText($"x{buildingSo.RequiredAmount}");
    }
    
    private void Close()
    {

        _canvas.enabled = false;
        startProductionButton.onClick.RemoveAllListeners();
        removeProductionButton.onClick.RemoveAllListeners();
    }
}
