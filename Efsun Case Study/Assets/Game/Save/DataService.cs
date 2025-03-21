using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class DataService : IInitializable
{
    private readonly List<ISaveable> _saveables;
    private Data _data;

    private static string SaveFilePath => Application.persistentDataPath + "/save.save";

    [Inject]
    public DataService(List<ISaveable> saveables)
    {
        _saveables = saveables;
    }

    public async void Initialize()
    {
        await LoadDataAndNextSceneAsync();
    }

    private async UniTask LoadDataAndNextSceneAsync()
    {
        if (File.Exists(SaveFilePath))
        {
            var json = await File.ReadAllTextAsync(SaveFilePath);
            _data = JsonUtility.FromJson<Data>(json);
        }
        else _data = new Data();

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        await SceneManager.LoadSceneAsync(nextSceneIndex);
        await UniTask.NextFrame(); 
        
        foreach (var saveable in _saveables)
        {
            saveable.LoadData(_data);
        }
        
    }

    public async UniTask SaveAsync()
    {
        foreach (var saveable in _saveables)
        {
            saveable.SaveData(_data);
        }

        string json = JsonUtility.ToJson(_data, true);
        await File.WriteAllTextAsync(SaveFilePath, json);
    }
}