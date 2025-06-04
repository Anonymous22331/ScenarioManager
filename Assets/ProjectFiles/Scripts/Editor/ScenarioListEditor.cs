using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

/// <summary>
// Кастомный эдитор для удобства работы
//
// Легко добавлять новые пункты/шаги, удобно работать со сценариями
/// </summary>

[CustomEditor(typeof(ScenarioList))]
public class ScenarioListEditor : Editor
{
    private SerializedProperty groupsProperty;
    private List<bool> groupFoldouts = new();

    private void OnEnable()
    {
        groupsProperty = serializedObject.FindProperty("groups");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Сценарий", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        ScenarioList scenarioList = (ScenarioList)target;

        for (int i = 0; i < groupsProperty.arraySize; i++)
        {
            if (groupFoldouts.Count <= i)
                groupFoldouts.Add(true);

            SerializedProperty groupProp = groupsProperty.GetArrayElementAtIndex(i);
            SerializedProperty nameProp = groupProp.FindPropertyRelative("name");
            SerializedProperty stepsProp = groupProp.FindPropertyRelative("steps");

            groupFoldouts[i] = EditorGUILayout.Foldout(groupFoldouts[i], $"Группа {i + 1}: {nameProp.stringValue}", true);

            if (groupFoldouts[i])
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(nameProp, new GUIContent("Название группы"));

                for (int j = 0; j < stepsProp.arraySize; j++)
                {
                    SerializedProperty stepProp = stepsProp.GetArrayElementAtIndex(j);
                    SerializedProperty idProp = stepProp.FindPropertyRelative("id");
                    SerializedProperty descriptionProp = stepProp.FindPropertyRelative("description");
                    SerializedProperty actionProp = stepProp.FindPropertyRelative("action");
                    SerializedProperty handlerProp = stepProp.FindPropertyRelative("actionHandler");

                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.LabelField($"Шаг {j + 1}", EditorStyles.boldLabel);

                    EditorGUILayout.PropertyField(idProp);
                    EditorGUILayout.PropertyField(descriptionProp);
                    EditorGUILayout.PropertyField(actionProp);

                    // Фильтрация доступных IStepAction по StepActionType
                    StepActionType stepType = (StepActionType)actionProp.enumValueIndex;
                    var allHandlers = FindObjectsOfType<MonoBehaviour>(true)
                        .Where(mb => mb is IStepAction action && action.ActionType == stepType)
                        .ToList();
                    // Не стал больше расширять. Оставил только GetInstanceID для поиска нужного объекта.
                    // Не самый удобный вид идентификации, но лучше, чем ничего
                    var options = allHandlers.Select(h => $"{h.gameObject.name} ({h.GetInstanceID()})").ToArray();
                    MonoBehaviour currentHandler = handlerProp.objectReferenceValue as MonoBehaviour;
                    int selectedIndex = allHandlers.IndexOf(currentHandler);
                    int newIndex = EditorGUILayout.Popup("Обработчик действия", selectedIndex, options);

                    if (newIndex >= 0 && newIndex < allHandlers.Count)
                    {
                        handlerProp.objectReferenceValue = allHandlers[newIndex];
                    }
                    else if (allHandlers.Count == 0)
                    {
                        EditorGUILayout.HelpBox($"Нет компонентов IStepAction с типом {stepType} на объекте.", MessageType.Warning);
                        handlerProp.objectReferenceValue = null;
                    }

                    if (GUILayout.Button("Удалить шаг"))
                    {
                        stepsProp.DeleteArrayElementAtIndex(j);
                        break;
                    }

                    EditorGUILayout.EndVertical();
                }

                if (GUILayout.Button("Добавить шаг"))
                {
                    stepsProp.InsertArrayElementAtIndex(stepsProp.arraySize);
                }

                EditorGUILayout.Space();

                if (GUILayout.Button("Удалить группу"))
                {
                    groupsProperty.DeleteArrayElementAtIndex(i);
                    groupFoldouts.RemoveAt(i);
                    break;
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
        }

        if (GUILayout.Button("Добавить новую группу"))
        {
            groupsProperty.InsertArrayElementAtIndex(groupsProperty.arraySize);
            groupFoldouts.Add(true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
