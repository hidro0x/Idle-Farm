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
    [SerializeField]private Slider slider;
    
    public RectTransform Rect { get; private set; }
    private Canvas _canvas;
    
    private CompositeDisposable _disposables = new CompositeDisposable();

    private void Awake()
    {
        Rect = GetComponent<RectTransform>();
        _canvas = GetComponent<Canvas>();
    }

    /// <summary>
    /// BuildingSO'dan gelen yapı bilgileri ve reaktif BuildingData ile UI öğelerini günceller.
    /// </summary>
    public void Init(Building building)
    {

        _disposables.Clear();

        resourceIconImage.sprite = building.OutputResource.Icon;
        
        building.CurrentOrderAmount
            .Subscribe(_ =>
            {
                productionCountText.text = building.CurrentTotalCapacity > 0 ? $"{building.CurrentTotalCapacity}/{building.MaxCapacity}" : String.Empty;
            })
            .AddTo(_disposables);
        
        building.TimeLeft
            .Subscribe(time =>
            {
                if (building.TimeLeft.Value > 0)
                {
                    productionTimeText.text = $"{time:0.0} sn";
                    slider.value = time / building.ProductionTime;
                }
                else
                {
                    slider.value = 1;
                    productionTimeText.text = $"{building.ProductionTime:0.0} sn";
                }
            })
            .AddTo(_disposables);
        
        building.CurrentResourceAmount
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

