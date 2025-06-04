using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabAction : MonoBehaviour, IStepAction
{
    public StepActionType ActionType => StepActionType.Grab;

    private Step _step;
    private Action<Step> _onComplete;
    private bool _isCompleted;
    
    public void Init(Step step, Action<Step> onComplete)
    {
        this._step = step;
        this._onComplete = onComplete;

        var grab = this.GetComponent<XRGrabInteractable>();
        if (grab != null)
            grab.selectEntered.AddListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (_isCompleted) return;
        _isCompleted = true;
        _onComplete?.Invoke(_step);
    }
}