using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InteractObjectAction : MonoBehaviour, IStepAction
{
    public StepActionType ActionType => StepActionType.InteractObject;

    private Step _step;
    private Action<Step> _onComplete;
    private bool _isCompleted;


    public void Init(Step step, Action<Step> onComplete)
    {
        this._step = step;
        this._onComplete = onComplete;

        var interactable = this.GetComponent<XRBaseInteractable>();
        if (interactable != null)
            interactable.activated.AddListener(OnInteracted);
    }

    private void OnInteracted(ActivateEventArgs args)
    {
        if (_isCompleted) return;
        _isCompleted = true;
        _onComplete?.Invoke(_step);
    }
}