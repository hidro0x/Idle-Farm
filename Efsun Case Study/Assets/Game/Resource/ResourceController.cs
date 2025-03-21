using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ResourceController : IDisposable, ISaveable
{
    private Dictionary<ResourceSO, ReactiveProperty<int>> _resources = new();
    private readonly CompositeDisposable _resourceControlSubscriptions = new CompositeDisposable();

    private static readonly Subject<(ResourceSO resource, int amount)> OnResourceAddRequested =
        new Subject<(ResourceSO, int)>();

    private static readonly Subject<(ResourceSO resource, int amount)> OnResourceRemoveRequested =
        new Subject<(ResourceSO, int)>();

    public Dictionary<ResourceSO, ReactiveProperty<int>> Resources => _resources;

    [Inject]
    public ResourceController(GameSettings settings, DataService dataService)
    {
        dataService.Register(this);
        foreach (var resource in settings.Resources)
        {
            _resources.Add(resource.Key, new ReactiveProperty<int>(resource.Value));
        }

        OnResourceAddRequested
            .Subscribe(tuple => AddResource(tuple.resource, tuple.amount)).AddTo(_resourceControlSubscriptions);
        OnResourceRemoveRequested
            .Subscribe(tuple => RemoveResource(tuple.resource, tuple.amount)).AddTo(_resourceControlSubscriptions);
        
    }
    
    public void SetData(Dictionary<int, int> data)
    {
        foreach (var resource in data)
        {
            _resources.First(x=> x.Key.ID == resource.Key).Value.Value = resource.Value;
        }
    }
    
    public Dictionary<int, int> GetData()
    {
        var newData = new Dictionary<int, int>();
        foreach (var resource in _resources)
        {
            newData.Add(resource.Key.ID, resource.Value.Value);
        }

        return newData;
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

    public void LoadData(Data data)
    {
        foreach (var entry in data.ResourceDatas)
        {
            var matchingResource = _resources.Keys.FirstOrDefault(r => r.ID == entry.Key);
            if (matchingResource != null)
            {
                _resources[matchingResource].Value = entry.Value;
            }
        }
    }

    public void SaveData(Data data)
    {
        foreach (var resource in _resources)
        {
            if (data.ResourceDatas.ContainsKey(resource.Key.ID))
            {
                data.ResourceDatas[resource.Key.ID] = resource.Value.Value;
            }
            else
            {
                data.ResourceDatas.Add(resource.Key.ID, resource.Value.Value);
            }
            
        }
    }
}

