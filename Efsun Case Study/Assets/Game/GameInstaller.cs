using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private GameSettings gameSettings;

    public override void InstallBindings()
    {
        Debug.Log("✅ GameInstaller başladı.");
        Container.BindInterfacesAndSelfTo<DataService>().AsSingle().NonLazy();
        Container.Bind<GameSettings>().FromScriptableObject(gameSettings).AsSingle();
    }
}