using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Building : SerializedMonoBehaviour
{
    [SerializeField] private BuildingSO building;
    private BuildingData _data;

    public void Tick(float tickValue)
    {
        
    }

}
