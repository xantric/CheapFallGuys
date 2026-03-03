using UnityEngine;
using UnityEditor;

public class AddPrefabAsChild : EditorWindow
{
    private GameObject prefab;
    private Vector3 localScale = Vector3.one;
    private Vector3 localPosition = Vector3.zero;

    [MenuItem("Tools/Add Prefab As Child")]
    public static void ShowWindow()
    {
        GetWindow<AddPrefabAsChild>("Add Prefab As Child");
    }

    private void OnGUI()
    {
        GUILayout.Label("Prefab Settings", EditorStyles.boldLabel);

        prefab = (GameObject)EditorGUILayout.ObjectField(
            "Prefab",
            prefab,
            typeof(GameObject),
            false
        );

        localPosition = EditorGUILayout.Vector3Field("Local Position", localPosition);
        localScale = EditorGUILayout.Vector3Field("Local Scale", localScale);

        EditorGUILayout.Space();

        if (GUILayout.Button("Add To Selected Objects"))
        {
            AddPrefabToSelected();
        }
    }

    private void AddPrefabToSelected()
    {
        if (prefab == null)
        {
            Debug.LogError("Please assign a prefab.");
            return;
        }

        foreach (GameObject obj in Selection.gameObjects)
        {
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.transform.SetParent(obj.transform);
            instance.transform.localPosition = localPosition;
            instance.transform.localRotation = Quaternion.identity;
            instance.transform.localScale = localScale;

            Undo.RegisterCreatedObjectUndo(instance, "Add Prefab As Child");
        }
    }
}