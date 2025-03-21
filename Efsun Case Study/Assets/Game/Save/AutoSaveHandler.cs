using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;

public class AutoSaveHandler : MonoBehaviour
{
    [Inject] private DataService _dataService;

    private void OnApplicationQuit()
    {
        _ = _dataService.SaveAsync();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            _ = _dataService.SaveAsync();
        }
    }
}