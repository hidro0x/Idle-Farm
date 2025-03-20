using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using YigitDurmus;

public class ProductionButtonsUI : MonoBehaviour
{
    [SerializeField] private Button startProductionButton, removeProductionButton;
    [SerializeField] private Image resourceIcon;
    [SerializeField] private TextMeshProUGUI resourceRequiredAmountText;
    
    public static readonly Subject<BuildingObject> OnBuildingUIRequested = new Subject<BuildingObject>();

    private readonly CompositeDisposable _disposables = new CompositeDisposable();

    private RectTransform _rect;
    private Canvas _canvas;
    private BuildingObject _buildingObject;
    private bool IsOpen => _buildingObject != null;
    private bool IsSame(BuildingObject buildingObject) => buildingObject == _buildingObject;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _canvas = GetComponent<Canvas>();
    }
    
    private void OnEnable()
    {
        MobileTouchCamera.OnClickedOut
            .Subscribe(_ => Close()) // Paneli kapat
            .AddTo(_disposables);

        OnBuildingUIRequested
            .Subscribe(Open)
            .AddTo(_disposables);
    }

    private void OnDisable()
    {
        _disposables?.Clear();
    }
    

    private void Open(BuildingObject buildingObject)
    {
        
        if (IsOpen && IsSame(buildingObject))
        {
            buildingObject.Building.CollectResource();
            return;
        }

        _buildingObject = buildingObject;
        
        startProductionButton.onClick.RemoveAllListeners();
        removeProductionButton.onClick.RemoveAllListeners();
        
        startProductionButton.onClick.AddListener(buildingObject.Building.AddOrder);
        removeProductionButton.onClick.AddListener(buildingObject.Building.RemoveOrder);

        _rect.position = buildingObject.InfoUI.Rect.position;
        _canvas.enabled = true;
        
        resourceIcon.sprite = buildingObject.Building.InputResource.Icon;
        resourceRequiredAmountText.SetText($"x{buildingObject.Building.InputAmount}");
    }
    
    private void Close()
    {
        _canvas.enabled = false;
    }
}
