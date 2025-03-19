using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameSettings : SerializedScriptableObject
{
    [field:SerializeField] public float TickValue { get; private set; }
}
