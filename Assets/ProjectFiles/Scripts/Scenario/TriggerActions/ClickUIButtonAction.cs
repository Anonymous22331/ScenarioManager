using System;
using UnityEngine;
using UnityEngine.UI;

public class ClickUIButtonAction : MonoBehaviour, IStepAction
{
    public StepActionType ActionType => StepActionType.ClickUIButton;

    private Step step;
    private Action<Step> onComplete;
    private bool _isCompleted;
    
    public void Init(Step step, Action<Step> onComplete)
    {
        this.step = step;
        this.onComplete = onComplete;

        var button = this.GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        if (_isCompleted) return;
        _isCompleted = true;
        onComplete?.Invoke(step);
    }
}