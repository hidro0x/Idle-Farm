using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

public class BuildingSO : SerializedScriptableObject
{
    [field: Title("General Information")]
    [field: MinValue(1)]
    [field: SerializeField]
    public int Id { get; private set; }

    [field: Tooltip("If its building that not require any input resource.")]
    [field: OnValueChanged("SetAsGenerator")]
    [field: SerializeField]
    public bool IsGenerator { get; private set; }

    [field: Title("Building Prefab")]
    [field: PreviewField(100, ObjectFieldAlignment.Center)]
    [field: Required("Can not be empty.")]
    [field: SerializeField]
    public GameObject BuildingPrefab { get; private set; }

    [field: Title("Resources")]
    [field: Required("Can not be empty.")]
    [field: SerializeField]
    public ResourceSO OutputResource { get; private set; }

    [field: HideIf("IsGenerator")]
    [field: Required("Can not be empty.")]
    [field: SerializeField]
    public ResourceSO InputResource { get; private set; }

    [field: Title("Production Details")]
    [field: MinValue(1)]
    [field: SerializeField]
    public int BaseCapacity { get; private set; }

    [field: MinValue(1)]
    [field: SerializeField]
    public float BaseProductionTime { get; private set; }

    [field: HideIf("IsGenerator")]
    [field: MinValue(1)]
    [field: SerializeField]
    public int BaseProductionInputAmount { get; private set; }

    [field: SerializeField] private bool editOutputValue;

    [field: MinValue(1)]
    [field: EnableIf("editOutputValue")]
    [field: ValidateInput("IsOverCapacity", "Can't be bigger number than maximum capacity.")]
    [SerializeField]
    private int baseProductionOutputAmount = 1;

    public int BaseProductionOutputAmount => baseProductionOutputAmount;

    private bool IsOverCapacity(int num)
    {
        return BaseProductionOutputAmount <= BaseCapacity;
    }

    private void SetAsGenerator()
    {
        BaseProductionInputAmount = 0;
        InputResource = null;
    }
}