using UnityEngine;
using UniRx;
using System;
using Zenject;

public class TimeController 
{
    public float TickValue { get; private set; }
    public IObservable<float> OnTick { get; private set; }

    [Inject]
    public TimeController(GameSettings gameSettings)
    {
        TickValue = gameSettings.TickValue;
        OnTick = Observable.Interval(TimeSpan.FromSeconds(1))
            .Select(_ => TickValue).Publish().RefCount();
    }
    
}
