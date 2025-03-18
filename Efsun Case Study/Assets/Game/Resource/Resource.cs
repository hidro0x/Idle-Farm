using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Resource : SerializedScriptableObject
{
    [SerializeField] private int id;
    [SerializeField] private string name;
    [SerializeField] private Sprite icon;

    public string Name => name;
    public int ID => id;
    public Sprite Icon => icon;

}
