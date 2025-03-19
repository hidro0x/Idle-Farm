using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class InfoSliderUI : MonoBehaviour
{
    [SerializeField] private Image resourceIconImage;
    [SerializeField] private TextMeshProUGUI productionCountText, productionTimeText, resourceCountText;
    private Slider _slider;
    
    public RectTransform Rect { get; private set; }
    private Canvas _canvas;
    
    private CompositeDisposable _disposables = new CompositeDisposable();

    private void Awake()
    {
        _slider = GetComponentInChildren<Slider>();
        Rect = GetComponent<RectTransform>();
        _canvas = GetComponent<Canvas>();
    }

    /// <summary>
    /// BuildingSO'dan gelen yapı bilgileri ve reaktif BuildingData ile UI öğelerini günceller.
    /// </summary>
    public void Init(Building building)
    {
        var buildingSO = building.BuildingSO;
        var data = building.Data;
        
        _disposables.Clear();
        
        resourceIconImage.sprite = buildingSO.ResourceSo.Icon;
        
        data.CurrentCapacity
            .Subscribe(capacity => {
                productionCountText.text = $"{capacity}/{buildingSO.Capacity}";
            })
            .AddTo(_disposables);
        
        data.TimeLeft
            .Subscribe(time =>
            {
                productionTimeText.text = $"{time:0.0} sn";
                _slider.value = time / buildingSO.ProductionTime;
            })
            .AddTo(_disposables);
        
        data.CurrentResourceAmount
            .Subscribe(amount => resourceCountText.text = amount.ToString())
            .AddTo(_disposables);
    }

    public void Show()
    {
        _canvas.enabled = true;
    }
    
    public void Close()
    {
        _canvas.enabled = false;
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}

public class InfoSliderUIFactory : PlaceholderFactory<InfoSliderUI> { }
