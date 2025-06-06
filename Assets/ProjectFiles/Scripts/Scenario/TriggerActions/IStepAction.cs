using System;

/// <summary>
// 
// Интерфейс для реализации различных видов шагов
//
/// </summary>


public interface IStepAction
{
    StepActionType ActionType { get; }
    
    void Init(Step step, Action<Step> onStepComplete);

    void InvokeStepCompleteEvents();
}