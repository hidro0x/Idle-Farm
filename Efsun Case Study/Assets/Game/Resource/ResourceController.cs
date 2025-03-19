using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ResourceController : MonoBehaviour
{
    private Dictionary<ResourceSO, ReactiveProperty<int>> _resources = new Dictionary<ResourceSO, ReactiveProperty<int>>();
    
    public static readonly Subject<(ResourceSO resource, int amount)> OnResourceAddRequested = new Subject<(ResourceSO, int)>();

    private IDisposable _resourceAddSubscription;

    [Inject]
    public void Construct(GameSettings settings, HorizontalLayoutGroup spawnTransform, ResourceBarUI prefab)
    {
        foreach (var resource in settings.Resources)
        {
            var kvp = new KeyValuePair<ResourceSO, ReactiveProperty<int>>(resource.Key, new ReactiveProperty<int>());
            _resources.Add(kvp.Key, kvp.Value);
            _resources[resource.Key].Value = resource.Value;
            var slot =Instantiate(prefab, spawnTransform.transform);
            slot.Init(kvp);
        }
    }

    private void OnEnable()
    {
        _resourceAddSubscription = OnResourceAddRequested
            .Subscribe(tuple => AddResource(tuple.resource, tuple.amount));
    }

    private void OnDisable()
    {
        _resourceAddSubscription?.Dispose();
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
