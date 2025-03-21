using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameSettings : SerializedScriptableObject
{
    [field: SerializeField] public float TickValue { get; private set; }
    [field: SerializeField] private bool editStartingValues;

    [field: EnableIf("editStartingValues")]
    [field: DictionaryDrawerSettings(KeyLabel = "Resource SO", ValueLabel = "Start Value")]
    [SerializeField]
    private Dictionary<ResourceSO, int> resources = new Dictionary<ResourceSO, int>();

    public Dictionary<ResourceSO, int> Resources => resources;

    [Button]
    private void RefreshSettings()
    {
        DefaultTickValue();
        GetAndSortAllResources();
    }

    private void DefaultTickValue()
    {
        TickValue = 1f;
    }

    private void GetAndSortAllResources()
    {
        resources.Clear();
        var temp = UnityEngine.Resources.LoadAll<ResourceSO>("ResourceDatabase");

        foreach (var so in temp)
        {
            resources.Add(so, 0);
        }

        var sortedByValue = resources.OrderBy(kvp => kvp.Value).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        resources = sortedByValue;
    }
}