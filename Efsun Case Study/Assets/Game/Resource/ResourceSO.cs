using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ResourceSO : SerializedScriptableObject
{
    [SerializeField] private int id;
    [SerializeField] private string name;
    [PreviewField(100)] [SerializeField] private Sprite icon;

    public string Name => name;
    public int ID => id;
    public Sprite Icon => icon;
}