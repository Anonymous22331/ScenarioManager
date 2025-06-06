using UnityEngine;
using UnityEditor;

/// <summary>
// 
// Добавление InstanceID объектов, для удобства работы
//
/// </summary>


[InitializeOnLoad]
public static class GameObjectInstanceIDViewer
{
    static GameObjectInstanceIDViewer()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
    }

    private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (go == null)
            return;
        
        Rect rect = new Rect(selectionRect);
        rect.xMin = rect.xMax - 100;

        GUIStyle style = new GUIStyle(EditorStyles.label);
        style.normal.textColor = Color.gray;
        style.alignment = TextAnchor.MiddleRight;
        GUI.Label(rect, $"ID: {instanceID}", style);
    }
}