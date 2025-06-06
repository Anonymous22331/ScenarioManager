using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketAction : MonoBehaviour, IStepAction
{
    [SerializeField] private GameObject _targetObject;
    [SerializeField] private UnityEvent _onStepCompleteEvents;
    public StepActionType ActionType => StepActionType.Socket;

    private Step _step;
    private Action<Step> _onComplete;
    private bool _isCompleted;
    
    public void Init(Step step, Action<Step> onComplete)
    {
        this._step = step;
        this._onComplete = onComplete;

        var socket = GetComponent<XRSocketInteractor>();
        if (socket != null)
            socket.selectEntered.AddListener(OnObjectSocketed);
    }

    public void InvokeStepCompleteEvents()
    {
        _onStepCompleteEvents.Invoke();
    }

    private void OnObjectSocketed(SelectEnterEventArgs args)
    {
        if (_isCompleted) return;
        if (args.interactableObject.transform.gameObject == _targetObject)
        {
            _isCompleted = true;
            _onComplete?.Invoke(_step);
        }
    }
}