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
    private readonly CompositeDisposable _resourceControlSubscriptions = new CompositeDisposable();
    
    private static readonly Subject<(ResourceSO resource, int amount)> OnResourceAddRequested = new Subject<(ResourceSO, int)>();
    private static readonly Subject<(ResourceSO resource, int amount)> OnResourceRemoveRequested = new Subject<(ResourceSO, int)>();
    public Dictionary<ResourceSO, ReactiveProperty<int>> Resources => _resources;

    [Inject]
    public ResourceController(GameSettings settings)
    {
        foreach (var resource in settings.Resources)
        {
            _resources.Add(resource.Key, new ReactiveProperty<int>(resource.Value));
        }

        OnResourceAddRequested
            .Subscribe(tuple => AddResource(tuple.resource, tuple.amount)).AddTo(_resourceControlSubscriptions);
        OnResourceRemoveRequested
            .Subscribe(tuple => RemoveResource(tuple.resource, tuple.amount)).AddTo(_resourceControlSubscriptions);
    }

    public void Dispose()
    {
        _resourceControlSubscriptions.Clear(); 
    }

    private void AddResource(ResourceSO resource, int amountToAdd)
    {
        _resources[resource].Value += amountToAdd;
    }
    
    private void RemoveResource(ResourceSO resource, int amountToRemove)
    {
        if (_resources[resource].Value - amountToRemove < 0)
        {
            _resources[resource].Value = 0;
            return;
        }
        _resources[resource].Value -= amountToRemove;
    }

    public static void RequestRemoveResource(ResourceSO resource, int amount)
    {
        OnResourceRemoveRequested.OnNext((resource, amount));
    }
    
    public static void RequestAddResource(ResourceSO resource, int amount)
    {
        OnResourceAddRequested.OnNext((resource, amount));
    }
}
