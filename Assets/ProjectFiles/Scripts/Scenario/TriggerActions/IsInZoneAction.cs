using System;
using UnityEngine;
using UnityEngine.Events;

public class IsInZoneAction : MonoBehaviour, IStepAction
{
    [SerializeField] private GameObject _targetObject;
    
    public StepActionType ActionType => StepActionType.IsInZone;

    [SerializeField] private UnityEvent _onStepCompleteEvents;
    
    private Step _step;
    private Action<Step> _onComplete;
    private bool _isCompleted;

    public void Init(Step step, Action<Step> onComplete)
    {
        _step = step;
        _onComplete = onComplete;
    }

    public void InvokeStepCompleteEvents()
    {
        _onStepCompleteEvents.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isCompleted || _step == null) return;
        if (other.gameObject == _targetObject)
        {
            _isCompleted = true;
            _onComplete?.Invoke(_step);
        }
    }
}