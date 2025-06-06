using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
// 
// Основной контроллер сценария, запускает ивенты, двигается по сценарию
//
/// </summary>

public class ScenarioController : MonoBehaviour
{
    [SerializeField] private ScenarioList _scenarioList;

    private int _currentGroupIndex = 0;
    private HashSet<int> _completedStepIds = new();
    
    private StepGroup _currentGroup;
    public StepGroup CurrentStepGroup => _currentGroup;
    
    private Step _currentStep;
    public Step CurrentStep => _currentStep;
    
    public event Action<Step> OnStepStarted;
    public event Action<Step> OnStepCompleted;
    public event Action<StepGroup> OnGroupCompleted;
    public event Action OnScenarioCompleted;

    private bool _scenarioFinished;

    private void Start()
    {
        if (!IsScenarioValid())
        {
            Debug.LogError("ScenarioList не назначен или пуст.");
            return;
        }
        
        _currentGroupIndex = 0;
        SyncCurrentGroupAndStep();
        StartGroup();
    }

    private bool IsScenarioValid()
    {
        return _scenarioList != null &&
               _scenarioList.groups.Count > 0 &&
               _scenarioList.groups[0].steps.Count > 0;
    }
    
    private void SyncCurrentGroupAndStep()
    {
        if (_currentGroupIndex < _scenarioList.groups.Count)
        {
            _currentGroup = _scenarioList.groups[_currentGroupIndex];
            _currentStep  = _currentGroup.steps
                .FirstOrDefault(step => !_completedStepIds.Contains(step.id));
        }
        else
        {
            _currentGroup = null;
            _currentStep  = null;
        }
    }

    private void StartGroup()
    {
        if (_scenarioFinished) return;

        _completedStepIds.Clear();
        Debug.Log($"[ScenarioController] Группа '{_currentGroup.name}' запущена.");

        foreach (var step in _currentGroup.steps)
        {
            OnStepStarted?.Invoke(step);
            step.stepState = StepState.Pending;
            if (step.actionHandler is IStepAction action)
            {
                action.Init(step, TryCompleteStep);
            }
            else
            {
                Debug.LogError($"[ScenarioController] Объект для шага {step.id} не реализует IStepAction!");
            }
        }
    }
    
    private Step GetExpectedStep()
    { 
        return _currentGroup.steps.FirstOrDefault(step => !_completedStepIds.Contains(step.id));
    }
    
    private void TryCompleteStep(Step step)
    {
        if (_scenarioFinished || _completedStepIds.Contains(step.id)) return;
        if (!_currentGroup.steps.Contains(step)) return;
        
        var expectedStep = GetExpectedStep();
        if (expectedStep != null && expectedStep.id != step.id)
        {
            SkipCurrentGroup();
            return;
        }

        Debug.Log($"[ScenarioController] Шаг {step.id} завершён: {step.description}");
        
        _completedStepIds.Add(step.id);

        if (step.actionHandler is IStepAction action)
        {
            action.InvokeStepCompleteEvents();
        }
        step.stepState = StepState.Completed;
        
        OnStepCompleted?.Invoke(step);
        
        if (_completedStepIds.Count < _currentGroup.steps.Count)
        {
            SyncCurrentGroupAndStep();
        }
        else
        {
            Debug.Log($"[ScenarioController] Группа «{_currentGroup.name}» завершена.");
            OnGroupCompleted?.Invoke(_currentGroup);
            AdvanceToNextGroup();
        }
    }
    
    private void AdvanceToNextGroup()
    {
        _currentGroupIndex++;

        if (_currentGroupIndex >= _scenarioList.groups.Count)
        {
            Debug.Log("[ScenarioController] Все группы завершены.");
            _scenarioFinished = true;
            OnScenarioCompleted?.Invoke();
            return;
        }

        SyncCurrentGroupAndStep();
        StartGroup();
    }

    
    // Проигрываются все ивенты, которые были до, чтобы корректно запустить следующую группу
    // Можно использовать и для пропуска этапов в сценарии
    private void SkipCurrentGroup()
    {
        Debug.LogWarning($"[ScenarioController] Порядок нарушен — группа «{_currentGroup.name}» будет пропущена.");

        foreach (var step in _currentGroup.steps.Where(step => !_completedStepIds.Contains(step.id)))
        {
            _completedStepIds.Add(step.id);
            step.stepState = StepState.Failed;
            OnStepCompleted?.Invoke(step);

            if (step.actionHandler is IStepAction action)
            {
                action.InvokeStepCompleteEvents();
            }
        }

        OnGroupCompleted?.Invoke(_currentGroup);
        AdvanceToNextGroup();
    }
    
}
