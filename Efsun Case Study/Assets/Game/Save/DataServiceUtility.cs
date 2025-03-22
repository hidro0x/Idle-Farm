using UnityEditor;
using UnityEngine;
using System.IO;

#if  UNITY_EDITOR

public static class DataServiceUtility
{
    private static string SaveFilePath => Application.persistentDataPath + "/save.save";

    [MenuItem("Save/Delete Save File", priority = 0)]
    public static void DeleteSaveFile()
    {
        if (File.Exists(SaveFilePath))
        {
            File.Delete(SaveFilePath);
        }
        else
        {
            Debug.LogWarning("Kayıt dosyası bulunamadı.");
        }
    }
}
#endif
