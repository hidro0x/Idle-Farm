using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ResourceController : IDisposable
{
    private Dictionary<ResourceSO, ReactiveProperty<int>> _resources = new();
    private readonly IDisposable _resourceAddSubscription;
    public static readonly Subject<(ResourceSO resource, int amount)> OnResourceAddRequested = new Subject<(ResourceSO, int)>();
    public Dictionary<ResourceSO, ReactiveProperty<int>> Resources => _resources;

    [Inject]
    public ResourceController(GameSettings settings)
    {
        foreach (var resource in settings.Resources)
        {
            _resources.Add(resource.Key, new ReactiveProperty<int>(resource.Value));
        }

        _resourceAddSubscription = OnResourceAddRequested
            .Subscribe(tuple => AddResource(tuple.resource, tuple.amount));
    }

    public void Dispose()
    {
        _resourceAddSubscription.Dispose(); 
    }

    private void AddResource(ResourceSO resource, int amountToAdd)
    {
        _resources[resource].Value += amountToAdd;
    }
    
    private void RemoveResource(ResourceSO resource, int amountToRemove)
    {
        _resources[resource].Value -= amountToRemove;
    }

    public int GetResourceAmount(ResourceSO resource)
    {
        return _resources[resource].Value;
    }
}
