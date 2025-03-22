using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ResourceBarController : MonoBehaviour
{
    private List<ResourceBarUI> _resourceBars = new List<ResourceBarUI>();
    [SerializeField] private ResourceBarUI resourceBarUIPrefab;
    [SerializeField] private HorizontalLayoutGroup resourceBarSpawnArea;

    [Inject] private ResourceController _resourceController;


    private void Start()
    {

        foreach (var resource in _resourceController.Resources)
        {
            var slot = Instantiate(resourceBarUIPrefab, resourceBarSpawnArea.transform);
            slot.Init(resource);
            _resourceBars.Add(slot);
        }
        
    }
    
    

    public RectTransform GetBarForResource(ResourceSO resource)
    {
        foreach (var bar in _resourceBars)
        {
            if (bar.Resource == resource)
                return bar.IconTransform;
        }

        return null;
    }
}

