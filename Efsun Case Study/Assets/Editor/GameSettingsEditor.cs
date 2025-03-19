using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class GameSettingsEditor : OdinMenuEditorWindow
{
    [MenuItem("General Settings/Settings Database")]
    private static void OpenWindow() 
    { 
        GetWindow<GameSettingsEditor>().Show();
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();
        tree.Selection.SupportsMultiSelect = false;
        
        tree.Add("Create New Setting", new CreateNewSetting());
        tree.AddAllAssetsAtPath("Settings", "Assets/Resources/SettingsDatabase/", typeof(GameSettings));
        return tree;
    }
    
    
    public class CreateNewSetting
    {
        public CreateNewSetting()
        {
            settingData = CreateInstance<GameSettings>();
        }

        [InlineEditor(Expanded = true)]
        public GameSettings settingData;

        [Button("Create New Setting")]
        private void CreateNewData()
        {
            AssetDatabase.CreateAsset(settingData, "Assets/Resources/SettingsDatabase/" + "GameSetting" + ".asset");
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

