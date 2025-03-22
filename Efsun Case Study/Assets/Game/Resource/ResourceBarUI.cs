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
    private ResourceSO _resource;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Image iconImg;

    public RectTransform IconTransform => iconImg.rectTransform;
    public ResourceSO Resource => _resource;
    private IDisposable _disposable;
    

    public void Init(KeyValuePair<ResourceSO, ReactiveProperty<int>> kvp)
    {
        _disposable = kvp.Value.Subscribe(capacity => { amountText.text = $"{capacity}"; }).AddTo(this);
        iconImg.sprite = kvp.Key.Icon;
        _resource = kvp.Key;
    }


    private void OnDisable()
    {
        _disposable?.Dispose();
    }
}