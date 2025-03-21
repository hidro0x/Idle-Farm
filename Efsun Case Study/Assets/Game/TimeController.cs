using System;
using UniRx;
using Zenject;

public class TimeController : IDisposable
{
    private readonly Subject<float> _onTick = new Subject<float>();
    public IObservable<float> OnTick => _onTick;
    
    private readonly float _tickValue;
    private readonly IDisposable _tickSubscription;

    [Inject]
    public TimeController(GameSettings gameSettings)
    {
        _tickValue = gameSettings.TickValue; 
        
        _tickSubscription = Observable.Interval(TimeSpan.FromSeconds(_tickValue))
            .Subscribe(_ => _onTick.OnNext(_tickValue));
    }
    
    public void Dispose()
    {
        _tickSubscription.Dispose();
        _onTick.Dispose();
    }
}


//Burada Update bazli bir yapi kurulmasi gerektigi zaman tercih edilebilir, fakat ben daha iyi olsun diye tick based bir yapi denemek istedim ki oyunlar telefonlarda
//kimi zaman pause state aldiginda ufak da olsa arkaplandaki offline progression da hesaplanabilir.
/*public class TimeController : ITickable, IDisposable
{
    private readonly Subject<float> _onTick = new Subject<float>();
    public IObservable<float> OnTick => _onTick;

    public void Tick()
    {
        _onTick.OnNext(Time.deltaTime);
    }

    public void Dispose()
    {
        _onTick?.Dispose();
    }
}*/
