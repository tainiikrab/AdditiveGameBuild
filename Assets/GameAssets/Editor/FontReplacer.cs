using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

public class FontReplacer : EditorWindow
{
    private TMP_FontAsset newFont;

    private void OnGUI()
    {
        GUILayout.Label("Replace all TMP fonts in scene and prefabs", EditorStyles.boldLabel);

        newFont = (TMP_FontAsset)EditorGUILayout.ObjectField("New TMP Font", newFont, typeof(TMP_FontAsset), false);

        if (GUILayout.Button("Replace Fonts"))
        {
            if (newFont == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign a TMP_FontAsset first.", "OK");
                return;
            }

            ReplaceInScene();
            ReplaceInPrefabs();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Font replacement complete.");
        }
    }

    [MenuItem("Tools/TMP/Replace Fonts (Scene + Prefabs)")]
    public static void ShowWindow()
    {
        GetWindow<FontReplacer>("TMP Font Replacer");
    }

    private void ReplaceInScene()
    {
        var count = 0;

        var tmpros = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>()
            .Where(t => !EditorUtility.IsPersistent(t));
        foreach (var tmp in tmpros)
        {
            Undo.RecordObject(tmp, "Replace TMP Font");
            tmp.font = newFont;
            EditorUtility.SetDirty(tmp);
            count++;
        }

        var tmps3D = Resources.FindObjectsOfTypeAll<TextMeshPro>()
            .Where(t => !EditorUtility.IsPersistent(t));
        foreach (var tmp in tmps3D)
        {
            Undo.RecordObject(tmp, "Replace TMP Font");
            tmp.font = newFont;
            EditorUtility.SetDirty(tmp);
            count++;
        }

        Debug.Log($"Scene: replaced font on {count} TMP components.");
    }

    private void ReplaceInPrefabs()
    {
        var prefabGuids = AssetDatabase.FindAssets("t:Prefab");
        var count = 0;

        foreach (var guid in prefabGuids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) continue;

            var modified = false;

            foreach (var tmp in prefab.GetComponentsInChildren<TextMeshProUGUI>(true))
            {
                Undo.RecordObject(tmp, "Replace TMP Font");
                tmp.font = newFont;
                EditorUtility.SetDirty(tmp);
                modified = true;
                count++;
            }

            foreach (var tmp in prefab.GetComponentsInChildren<TextMeshPro>(true))
            {
                Undo.RecordObject(tmp, "Replace TMP Font");
                tmp.font = newFont;
                EditorUtility.SetDirty(tmp);
                modified = true;
                count++;
            }

            if (modified) PrefabUtility.SavePrefabAsset(prefab);
        }

        Debug.Log($"Prefabs: replaced font on {count} TMP components.");
    }
}