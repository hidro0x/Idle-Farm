using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using YigitDurmus;
using Zenject;

public class ProductionButtonsUI : MonoBehaviour
{
    [SerializeField] private Button startProductionButton, removeProductionButton;
    [SerializeField] private Image resourceIcon;
    [SerializeField] private TextMeshProUGUI resourceRequiredAmountText;
    
    private RectTransform _rect;
    private Canvas _canvas;
    
    public static readonly Subject<BuildingObject> OnBuildingUIRequested = new Subject<BuildingObject>();
    private readonly CompositeDisposable _disposables = new CompositeDisposable();
    private IDisposable _buttonSubscriptions;

    private BuildingObject _buildingObject;
    [Inject]private ResourceController _resourceController;
    private bool IsOpen => _buildingObject != null;
    private bool IsSame(BuildingObject buildingObject) => buildingObject == _buildingObject;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _canvas = GetComponent<Canvas>();
        
        AddHoldEffect(startProductionButton);
        AddHoldEffect(removeProductionButton);
        
    }
    
    private void OnEnable()
    {
        MobileTouchCamera.OnClickedOut
            .Subscribe(_ =>
            {
                Hide();
                _buildingObject = null;
            })
            .AddTo(_disposables);

        OnBuildingUIRequested
            .Subscribe(Init)
            .AddTo(_disposables);
    }

    private void OnDisable()
    {
        _disposables?.Clear();
        _buttonSubscriptions?.Dispose();
    }
    

    private void Init(BuildingObject buildingObject)
    {
        if (IsOpen && IsSame(buildingObject))
        {
            buildingObject.Building.CollectResource();
            return;
        }

        _buildingObject = buildingObject;
        _buttonSubscriptions?.Dispose();
        
        //Hem Resource kaynagina hem de building kapasitesine subs olarak add order buttonunu gunceller.
        _buttonSubscriptions = _resourceController.Resources[_buildingObject.Building.InputResource]
            .CombineLatest(_buildingObject.Building.CurrentOrderCapacity,
                (availableResource, currentCapacity) => _buildingObject.Building.CanAddOrder(availableResource))
            .Subscribe(canProduce =>
            {
                startProductionButton.interactable = canProduce;
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
    
    private void SetPosition(Vector3 pos) => _rect.position = pos;

    private void AddHoldEffect(Button button)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>() ?? button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry pointerDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        pointerDown.callback.AddListener(_ => StartButtonHold(button));

        EventTrigger.Entry pointerUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        pointerUp.callback.AddListener(_ => StopButtonHold(button));

        trigger.triggers.Add(pointerDown);
        trigger.triggers.Add(pointerUp);
    }
    
    private void StartButtonHold(Button button)
    {
        button.transform.DOScale(Vector3.one * 0.9f, 0.1f).SetEase(Ease.OutQuad);
    }
    
    private void StopButtonHold(Button button)
    {
        button.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutQuad);
    }
}
