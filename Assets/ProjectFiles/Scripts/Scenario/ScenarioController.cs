using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScenarioController : MonoBehaviour
{
    [SerializeField] private ScenarioList scenarioList;

    private int _currentGroupIndex = 0;
    private HashSet<int> _completedStepIds = new();
    private StepGroup currentGroup => scenarioList.groups[_currentGroupIndex];
    
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
        
        StartGroup();
    }

    private bool IsScenarioValid()
    {
        return scenarioList != null &&
               scenarioList.groups.Count > 0 &&
               scenarioList.groups[0].steps.Count > 0;
    }

    private void StartGroup()
    {
        if (_scenarioFinished) return;

        _completedStepIds.Clear();
        Debug.Log($"[Сценарий] Группа '{currentGroup.name}' запущена.");

        foreach (var step in currentGroup.steps)
        {
            OnStepStarted?.Invoke(step);
            if (step.actionHandler is IStepAction action)
            {
                action.Init(step, TryCompleteStep);
            }
            else
            {
                Debug.LogError($"[Сценарий] Объект для шага {step.id} не реализует IStepAction!");
            }
        }
    }


    public void TryCompleteStep(Step step)
    {
        if (_scenarioFinished || _completedStepIds.Contains(step.id)) return;
        if (!currentGroup.steps.Contains(step)) return;

        Debug.Log($"[Сценарий] Шаг {step.id} завершён: {step.description}");
        _completedStepIds.Add(step.id);
        OnStepCompleted?.Invoke(step);

        if (_completedStepIds.Count >= currentGroup.steps.Count)
        {
            Debug.Log($"[Сценарий] Группа '{currentGroup.name}' завершена.");
            OnGroupCompleted?.Invoke(currentGroup);
            AdvanceToNextGroup();
        }
    }

    private void AdvanceToNextGroup()
    {
        _currentGroupIndex++;

        if (_currentGroupIndex >= scenarioList.groups.Count)
        {
            Debug.Log("[Сценарий] Все группы завершены.");
            _scenarioFinished = true;
            OnScenarioCompleted?.Invoke();
            return;
        }

        StartGroup();
    }
}
