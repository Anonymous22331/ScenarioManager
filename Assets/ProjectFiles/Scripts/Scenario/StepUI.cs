using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
// 
// Скрипт для управления завершенностью шагов
//
/// </summary>

public class StepUI : MonoBehaviour
{
    [SerializeField] private GameObject _completedImg;
    [SerializeField] private GameObject _failedImg;

    public StepState State { get; private set; } = StepState.Pending;
    
    public void SetState(StepState state)
    {
        State = state;
        _completedImg.SetActive(state == StepState.Completed);
        _failedImg.SetActive(state == StepState.Failed);
    }
}
