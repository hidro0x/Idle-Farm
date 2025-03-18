using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class BuildingEditor : OdinMenuEditorWindow
{
    [MenuItem("Buildings/Building Database")]
    private static void OpenWindow() 
    { 
        GetWindow<BuildingEditor>().Show();
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();
        tree.Selection.SupportsMultiSelect = false;

        tree.Add("Create New Building", new CreateNewBuilding());
        tree.AddAllAssetsAtPath("Buildings", "Assets/Resources/BuildingDatabase/", typeof(BuildingSO));
        tree.Add("Create New Resource", new CreateNewResource());
        tree.AddAllAssetsAtPath("Resources", "Assets/Resources/ResourceDatabase/", typeof(ResourceSO));
        return tree;
    }

    public class CreateNewBuilding
    {
        public CreateNewBuilding()
        {
            buildingData = CreateInstance<BuildingSO>();
        }

        [InlineEditor(Expanded = true)]
        public BuildingSO buildingData;

        [Button("Create New Building")]
        private void CreateNewData()
        {
            AssetDatabase.CreateAsset(buildingData, "Assets/Resources/BuildingDatabase/" + buildingData.ID + ".asset");
            AssetDatabase.SaveAssets();
        }
    }
    
    public class CreateNewResource
    {
        public CreateNewResource()
        {
            resourceData = CreateInstance<ResourceSO>();
        }

        [InlineEditor(Expanded = true)]
        public ResourceSO resourceData;

        [Button("Create New Resource")]
        private void CreateNewData()
        {
            AssetDatabase.CreateAsset(resourceData, "Assets/Resources/ResourceDatabase/" + resourceData.ID + ".asset");
            AssetDatabase.SaveAssets();
        }
    }

    protected override void OnBeginDrawEditors()
    {
        OdinMenuTreeSelection selected = this.MenuTree.Selection;

        SirenixEditorGUI.BeginHorizontalToolbar();
        {
            GUILayout.FlexibleSpace();

            if (SirenixEditorGUI.ToolbarButton("Delete Current"))
            {
                var asset = selected.SelectedValue;
                string path = AssetDatabase.GetAssetPath((Object)asset);
                AssetDatabase.DeleteAsset(path);
                AssetDatabase.SaveAssets();
            }

        }
        SirenixEditorGUI.EndHorizontalToolbar(); 
    }
}

