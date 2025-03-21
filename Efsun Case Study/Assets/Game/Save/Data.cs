using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Data
{
    public Dictionary<int, int> ResourceDatas = new Dictionary<int, int>();
    public Dictionary<int, BuildingData> BuildingDatas = new Dictionary<int, BuildingData>();
}
