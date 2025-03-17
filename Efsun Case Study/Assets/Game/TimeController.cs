using UnityEngine;
using UniRx;
using System;

public class TimeController : MonoBehaviour
{
    public float tickValue = 1f;
    public IObservable<float> OnTick { get; private set; }

    void Start()
    {
        OnTick = Observable.Interval(TimeSpan.FromSeconds(1))
            .Select(_ => tickValue).Publish().RefCount();
    }
}
