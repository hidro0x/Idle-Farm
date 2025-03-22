using System;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "New Level", menuName = "Settings/New Level", order = 1)]
[Serializable]
public class LevelData : SerializedScriptableObject
{
    
    [field: HideInInspector]
    [field: SerializeField]private LevelGridBuilder levelGridBuilder;
    [field: Required("Can not be empty.")]
    [SerializeField] private GameObject groundObject;
    [field: Required("Can not be empty.")]
    [field:SerializeField] public BuildingObject BuildingPrefab{ get; private set; }
    [field:ShowIf("@this.Size != 0")]
    [field:Space]
    [field:Title("Level Details", TitleAlignment = TitleAlignments.Centered)]
    [field:ShowIf("@this.Size != 0")]
    [field: ReadOnly]
    [field: SerializeField]public int Spacing { get; private set; }
    [field:ShowIf("@this.Size != 0")]
    [field: ReadOnly]
    [field: SerializeField]public int Size{ get; private set; }

    public float CellSize
    {
        get
        {
            if (groundObject == null) return 0f;

            var col = groundObject.GetComponent<BoxCollider>();
            if (col != null)
            {
                return col.size.x * groundObject.transform.localScale.x;
            }

            return 0f;
        }
    }
    

    [TableMatrix(HorizontalTitle = "Board", DrawElementMethod = "DrawElement", ResizableColumns = false,
        RowHeight = 75, Transpose = true)]
    [ShowIf("@this.Size != 0 && this.levelGridBuilder != null")]
    public BuildingSO[,] Map = new BuildingSO[0, 0];
    
    
#if UNITY_EDITOR
    [ShowIf("@this.levelGridBuilder != null && this.groundObject != null && this.BuildingPrefab != null")]
    [Button(ButtonSizes.Medium, ButtonStyle.Box, Expanded = true)]
    private void CreateLevel(int size, int spacing)
    {
        Map = new BuildingSO[size, size];
        Size = size;
        Spacing = spacing;

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    
    [ShowIf("@this.levelGridBuilder == null")]
    [EnableIf("@this.groundObject != null && this.BuildingPrefab != null")]
    [Button(ButtonSizes.Medium, ButtonStyle.Box, Expanded = true)]
    private void FindOrCreateLevelBuilder()
    {
        if (!TryAssignGridBuilder())
        {
            levelGridBuilder = new GameObject("LevelGridBuilder").AddComponent<LevelGridBuilder>();
            levelGridBuilder.SetLevelData(this);
        }
        
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(levelGridBuilder.gameObject.scene);
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static int objectPickerID;
    private static Vector2Int selectedCell = new Vector2Int(-1, -1);

    private static BuildingSO DrawElement(Rect rect, BuildingSO value, BuildingSO[,] array, int y, int x, LevelData levelData)
{
    if (objectPickerID == 0)
        objectPickerID = GUIUtility.GetControlID(FocusType.Passive);

    if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
    {
        selectedCell = new Vector2Int(y, x);
        EditorGUIUtility.ShowObjectPicker<BuildingSO>(null, false, "", objectPickerID);
        Event.current.Use();
    }

    if (Event.current.commandName == "ObjectSelectorClosed" &&
        EditorGUIUtility.GetObjectPickerControlID() == objectPickerID)
    {
        BuildingSO selectedObject = EditorGUIUtility.GetObjectPickerObject() as BuildingSO;

        if (selectedCell != new Vector2Int(-1, -1))
        {
            if (selectedObject != null)
            {
                int newID = selectedObject.Id;

                bool duplicate = false;
                for (int i = 0; i < array.GetLength(0); i++)
                {
                    for (int j = 0; j < array.GetLength(1); j++)
                    {
                        if (array[i, j] != null && array[i, j].Id == newID && (i != selectedCell.x || j != selectedCell.y))
                        {
                            duplicate = true;
                            break;
                        }
                    }
                    if (duplicate) break;
                }

                if (duplicate)
                {
                    Debug.LogWarning($"'{newID}' ID'li bir obje zaten gridde mevcut!");
                }
                else
                {
                    array[selectedCell.x, selectedCell.y] = selectedObject;
                    levelData.levelGridBuilder.BuildGrid();
                    EditorUtility.SetDirty(levelData);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
            else
            {
                array[selectedCell.x, selectedCell.y] = null;
                levelData.levelGridBuilder.BuildGrid();
                EditorUtility.SetDirty(levelData);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        selectedCell = new Vector2Int(-1, -1); // reset
    }

    if (value != null)
    {
        var texture = AssetPreview.GetAssetPreview(value.BuildingPrefab);
        if (texture != null)
            EditorGUI.DrawPreviewTexture(rect.Padding(1), texture);
    }
    else
    {
        EditorGUI.DrawPreviewTexture(rect.Padding(1), Texture2D.normalTexture);
    }

    return array[y, x];
}
    
    private void OnEnable()
    {
        TryAssignGridBuilder();
    }

    private bool TryAssignGridBuilder()
    {
        var builder = FindObjectOfType<LevelGridBuilder>();
        if (builder != null)
        {
            builder.SetLevelData(this);
            levelGridBuilder = builder;
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(builder.gameObject.scene);
            return true;
        }
        
        return false;
    }


#endif
}