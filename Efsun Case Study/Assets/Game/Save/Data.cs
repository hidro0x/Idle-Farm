using System;
using System.Collections;
using System.Collections.Generic;
using Sherbert.Framework.Generic;
using UnityEngine;

[System.Serializable]
public class Data
{
    public SerializableDictionary<int, int> ResourceDatas = new SerializableDictionary<int, int>();
    public SerializableDictionary<int, BuildingData> BuildingDatas = new SerializableDictionary<int, BuildingData>();
    public DateTime LastSavedTime = DateTime.UtcNow;
}
