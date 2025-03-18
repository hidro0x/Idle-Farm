using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ResourceController : MonoBehaviour
{
    private Dictionary<int, ReactiveProperty<int>> _resources;
    
    public static readonly Subject<(Resource resource, int amount)> OnResourceAddRequested = new Subject<(Resource, int)>();

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

    private void AddResource(Resource resource, int amountToAdd)
    {
        _resources[resource.ID].Value += amountToAdd;
    }
    
    private void RemoveResource(Resource resource, int amountToRemove)
    {
        _resources[resource.ID].Value -= amountToRemove;
    }

    public int GetResourceAmount(Resource resource)
    {
        return _resources[resource.ID].Value;
    }
}
