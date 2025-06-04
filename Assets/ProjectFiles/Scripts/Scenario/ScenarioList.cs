using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}

public enum StepActionType
{
    IsInZone,
    Grab,
    Socket,
    InteractObject,
    ClickUIButton
}