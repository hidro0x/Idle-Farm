using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ResourceBarUI : MonoBehaviour
{
    [SerializeField] private ResourceSO resource;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Image iconImg;

    private IDisposable _disposable;

    [Inject]
    public void Construct(ResourceController resourceController)
    {
        Init(new KeyValuePair<ResourceSO, ReactiveProperty<int>>(resource, resourceController.Resources[resource]));
    }
    
    private void Init(KeyValuePair<ResourceSO, ReactiveProperty<int>> kvp)
    {
        _disposable = kvp.Value
            .Subscribe(capacity => {
                amountText.text = $"{capacity}";
            })
            .AddTo(this);
        iconImg.sprite = kvp.Key.Icon;
    }
    

    private void OnDisable()
    {
        _disposable.Dispose();
    }
}

