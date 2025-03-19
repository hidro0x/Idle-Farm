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
    
    public static readonly Subject<Building> OnBuildingUIRequested = new Subject<Building>();

    private readonly CompositeDisposable _disposables = new CompositeDisposable();

    private RectTransform _rect;
    private Canvas _canvas;

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
    

    private void Open(Building building)
    {
        startProductionButton.onClick.AddListener(building.AddToProductionOrder);
        removeProductionButton.onClick.AddListener(building.RemoveFromProductionOrder);

        _rect.position = building.InfoUI.Rect.position;
        _canvas.enabled = true;

        var buildingSo = building.BuildingSO;
        resourceIcon.sprite = buildingSo.Resource.Icon;
        resourceRequiredAmountText.SetText($"x{buildingSo.RequiredAmount}");
    }
    
    private void Close()
    {

        _canvas.enabled = false;
        startProductionButton.onClick.RemoveAllListeners();
        removeProductionButton.onClick.RemoveAllListeners();
    }
}
