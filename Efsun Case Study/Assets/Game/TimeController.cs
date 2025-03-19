using UnityEngine;
using UniRx;
using System;
using Zenject;

public class TimeController : MonoBehaviour
{
    private float _tickValue;
    public IObservable<float> OnTick { get; private set; }

    [Inject]
    public void Construct(GameSettings gameSettings)
    {
        _tickValue = gameSettings.TickValue;
    }
    
    void Start()
    {
        OnTick = Observable.Interval(TimeSpan.FromSeconds(1))
            .Select(_ => _tickValue).Publish().RefCount();
    }
}
