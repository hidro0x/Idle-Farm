using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ResourceController : MonoBehaviour
{
    private Dictionary<int, ReactiveProperty<int>> _resources = new Dictionary<int, ReactiveProperty<int>>();
    
    public static readonly Subject<(ResourceSO resource, int amount)> OnResourceAddRequested = new Subject<(ResourceSO, int)>();

    private IDisposable _resourceAddSubscription;

    private void OnEnable()
    {
        _resourceAddSubscription = OnResourceAddRequested
            .Subscribe(tuple => AddResource(tuple.resource, tuple.amount));
    }

    private void OnDisable()
    {
        _resourceAddSubscription?.Dispose();
    }

    private void AddResource(ResourceSO resourceSo, int amountToAdd)
    {
        _resources[resourceSo.ID].Value += amountToAdd;
    }
    
    private void RemoveResource(ResourceSO resourceSo, int amountToRemove)
    {
        _resources[resourceSo.ID].Value -= amountToRemove;
    }

    public int GetResourceAmount(ResourceSO resourceSo)
    {
        return _resources[resourceSo.ID].Value;
    }
}
