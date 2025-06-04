using System;

public interface IStepAction
{
    StepActionType ActionType { get; }
    
    void Init(Step step, Action<Step> onStepComplete);
}