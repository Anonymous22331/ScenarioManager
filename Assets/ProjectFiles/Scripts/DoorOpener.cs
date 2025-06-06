using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
// 
// Класс для управления дверьми
//
/// </summary>

public class DoorOpener : MonoBehaviour
{
    public enum OpenTriggerType { StepCompleted, GroupCompleted }

    [Header("Trigger Settings")]
    [SerializeField] private ScenarioController _scenario;
    [SerializeField] private ScenarioList _scenarioList;
    [SerializeField] private OpenTriggerType _openTriggerType = OpenTriggerType.StepCompleted;
    [SerializeField] private int _triggerIndex = 0;

    [Header("Door Target Transform")]
    [SerializeField] private Vector3 _targetPosition;
    [SerializeField] private Vector3 _targetEulerRotation;

    [Header("Animation Settings")]
    [SerializeField] private float _moveDuration = 1f;
    [SerializeField] private AnimationCurve _easing = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private bool _doorOpened = false;
    
    private Transform _doorTransform;

    // Есть выбор, открывается после шага или после группы шагов
    // Ниже реализация
    
    private void OnEnable()
    {
        if (_scenario == null)
        {
            Debug.LogWarning("DoorOpener: Missing references.");
            enabled = false;
            return;
        }

        _doorTransform = this.transform;

        _scenario.OnStepCompleted += HandleStepCompleted;
        _scenario.OnGroupCompleted += HandleGroupCompleted;
    }

    private void OnDisable()
    {
        _scenario.OnStepCompleted -= HandleStepCompleted;
        _scenario.OnGroupCompleted -= HandleGroupCompleted;
    }
    
    private void HandleStepCompleted(Step step)
    {
        if (_doorOpened || _openTriggerType != OpenTriggerType.StepCompleted || _scenarioList == null)
            return;

        int stepIndex = GetStepIndexInScenario(step.id);
        if (stepIndex == _triggerIndex)
        {
            OpenDoor();
        }
    }

    private void HandleGroupCompleted(StepGroup group)
    {
        if (_doorOpened || _openTriggerType != OpenTriggerType.GroupCompleted || _scenarioList == null)
            return;
        
        int groupIndex = _scenarioList.groups.IndexOf(group);
        if (groupIndex == _triggerIndex)
        {
            OpenDoor();
        }
    }

    private int GetStepIndexInScenario(int stepId)
    {
        int index = 0;
        foreach (var group in _scenarioList.groups)
        {
            foreach (var step in group.steps)
            {
                if (step.id == stepId)
                    return index;
                index++;
            }
        }
        return -1;
    }

    private void OpenDoor()
    {
        _doorOpened = true;
        StartCoroutine(AnimateDoorOpen());
    }

    private IEnumerator AnimateDoorOpen()
    {
        Vector3 startPos = _doorTransform.localPosition;
        Quaternion startRot = _doorTransform.localRotation;

        Vector3 endPos = _targetPosition;
        Quaternion endRot = Quaternion.Euler(_targetEulerRotation);

        float time = 0f;
        while (time < _moveDuration)
        {
            time += Time.deltaTime;
            float progress = _easing.Evaluate(time / _moveDuration);

            _doorTransform.localPosition = Vector3.Lerp(startPos, endPos, progress);
            _doorTransform.localRotation = Quaternion.Slerp(startRot, endRot, progress);

            yield return null;
        }

        _doorTransform.localPosition = endPos;
        _doorTransform.localRotation = endRot;
    }
}
