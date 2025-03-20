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
    private  IDisposable _buttonEvent;

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
            .Subscribe(_ => Hide()) // Paneli kapat
            .AddTo(_disposables);

        OnBuildingUIRequested
            .Subscribe(Init)
            .AddTo(_disposables);
    }

    private void OnDisable()
    {
        _disposables?.Clear();
        _buttonEvent.Dispose();
    }
    

    private void Init(BuildingObject buildingObject)
    {
        if (IsOpen && IsSame(buildingObject))
        {
            buildingObject.Building.CollectResource();
            return;
        }

        _buildingObject = buildingObject;
        
        _buttonEvent?.Dispose();
        _buttonEvent = buildingObject.Building.CurrentOrderCapacity
            .Subscribe(_ =>
            {
                startProductionButton.interactable = buildingObject.Building.CanAddOrder;
                removeProductionButton.interactable = buildingObject.Building.CanRemoveOrder;
            });
        
        startProductionButton.onClick.RemoveAllListeners();
        removeProductionButton.onClick.RemoveAllListeners();
        startProductionButton.onClick.AddListener(buildingObject.Building.AddOrder);
        removeProductionButton.onClick.AddListener(buildingObject.Building.RemoveOrder);
        
        resourceIcon.sprite = buildingObject.Building.InputResource.Icon;
        resourceRequiredAmountText.SetText($"x{buildingObject.Building.InputAmount}");
        
        SetPosition(buildingObject.InfoUI.Rect.position);
        Show();
    }
    
    private void Show() => _canvas.enabled = true;
    
    private void Hide() => _canvas.enabled = false;
 

    private void SetPosition(Vector3 pos)
    {
        _rect.position = pos;
    }
}
