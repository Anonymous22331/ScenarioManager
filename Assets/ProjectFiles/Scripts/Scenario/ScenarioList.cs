using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
// 
// Сам сценарий и его компоненты. Сделал удобный эдитор, чтобы было легче работать с ним
//
/// </summary>

public class ScenarioList : MonoBehaviour
{
    [Header("Группы шагов в этом сценарии")]
    public List<StepGroup> groups = new();
}


[System.Serializable]
public class StepGroup
{
    [Tooltip("Название группы шагов, например 'Проверка документов'")]
    public string name;

    [Tooltip("Список шагов в группе")]
    public List<Step> steps = new();
}

[System.Serializable]
public class Step
{
    [Tooltip("ID")]
    public int id;

    [TextArea]
    [Tooltip("Описание действия")]
    public string description;

    [Tooltip("Тип ожидаемого действия")]
    public StepActionType action;
    
    [Tooltip("Объект с данным действием")]
    public MonoBehaviour actionHandler;
    
    [HideInInspector] // Для получения состояния шага
    public StepState stepState;
}

public enum StepState
{ 
    Pending, 
    Completed, 
    Failed 
}

public enum StepActionType
{
    IsInZone,
    Grab,
    Socket,
    InteractObject,
    ClickUIButton
}