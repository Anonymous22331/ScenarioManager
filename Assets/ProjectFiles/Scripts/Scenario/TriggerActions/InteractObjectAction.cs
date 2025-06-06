using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class InteractObjectAction : MonoBehaviour, IStepAction
{
    public StepActionType ActionType => StepActionType.InteractObject;

    [SerializeField] private UnityEvent _onStepCompleteEvents;
    
    private Step _step;
    private Action<Step> _onComplete;
    private bool _isCompleted;


    public void Init(Step step, Action<Step> onComplete)
    {
        this._step = step;
        this._onComplete = onComplete;

        var interactable = this.GetComponent<XRSimpleInteractable>();
        if (interactable != null)
            interactable.activated.AddListener(OnInteracted);
    }

    public void InvokeStepCompleteEvents()
    {
        _onStepCompleteEvents.Invoke();
    }

    private void OnInteracted(ActivateEventArgs args)
    {
        if (_isCompleted) return;
        _isCompleted = true;
        _onComplete?.Invoke(_step);
    }
}