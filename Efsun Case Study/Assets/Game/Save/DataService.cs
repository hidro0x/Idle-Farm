using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using Formatting = System.Xml.Formatting;

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
    
    public void Initialize()
    {
        UniTask.Void(async () => { await LoadDataAndNextSceneAsync(); });
    }
    
    public void Register(ISaveable saveable)
    {
        if (!_saveables.Contains(saveable))
            _saveables.Add(saveable);
    }

    private async UniTask LoadDataAndNextSceneAsync()
    {
        if (File.Exists(SaveFilePath))
        {
            var json = await File.ReadAllTextAsync(SaveFilePath);
            _data = JsonConvert.DeserializeObject<Data>(json);
        }
        else _data = new Data();

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            await SceneManager.LoadSceneAsync(nextSceneIndex);
        }

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

        _data.LastSavedTime = DateTime.UtcNow;
        string json = JsonConvert.SerializeObject(_data, formatting: Newtonsoft.Json.Formatting.Indented);
        await File.WriteAllTextAsync(SaveFilePath, json);
    }

    
}