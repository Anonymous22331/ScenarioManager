using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
// 
// Контроллер для панели-помощника
//
/// </summary>

public class ScenarioUIHelper : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ScenarioController _scenario;
    [SerializeField] private GameObject _helpUIGameObject;
    
    [SerializeField] private TextMeshProUGUI _groupNameText;
    [SerializeField] private TextMeshProUGUI _stepDescriptionText;
    
    [SerializeField] private Transform _stepsRoot;
    [SerializeField] private GameObject _stepPrefab;
    
    [SerializeField] private List<Vector3> _positions;
    [SerializeField] private float _transitionDelay = 1f;

    private int _currentGroupIndex = 0;

    private Coroutine _stepCoroutine;
    private Coroutine _groupCoroutine;
    
    private bool _isTransitioningGroup = false;
    
    private CanvasGroup _canvasGroup;
    private CanvasGroup _descriptionCanvasGroup;

    private List<StepUI> _activeSteps = new();
    private List<GameObject> _stepPool = new();
    private Dictionary<int, StepState> _stepStates = new();

    private bool _groupInitialized = false;
    private StepGroup _deferredStepGroup = null;

    
    // Кавас группы для красивого перехода
    
    private void Awake()
    {
        _canvasGroup = _helpUIGameObject.GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = _helpUIGameObject.AddComponent<CanvasGroup>();
        
        _descriptionCanvasGroup = _stepDescriptionText.GetComponent<CanvasGroup>();
        if (_descriptionCanvasGroup == null)
            _descriptionCanvasGroup = _stepDescriptionText.gameObject.AddComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        _scenario.OnStepStarted += HandleStepStarted;
        _scenario.OnStepCompleted += HandleStepCompleted;
        _scenario.OnGroupCompleted += HandleGroupCompleted;
    }

    private void OnDisable()
    {
        _scenario.OnStepStarted -= HandleStepStarted;
        _scenario.OnStepCompleted -= HandleStepCompleted;
        _scenario.OnGroupCompleted -= HandleGroupCompleted;
    }

    private void HandleStepStarted(Step step)
    {
        
        if (_isTransitioningGroup)
        {
            _deferredStepGroup = _scenario.CurrentStepGroup;
            return;
        }
        
        if (!_groupInitialized)
        {
            SetupStepGroup(_scenario.CurrentStepGroup);
            _groupInitialized = true;
        }
        
        if (_scenario.CurrentStep != null && _scenario.CurrentStep.id == step.id)
        {
            _stepDescriptionText.text = step.description;
        }
    }


    private void HandleStepCompleted(Step step)
    {
        _stepStates[step.id] = StepState.Completed;

        for (int i = 0; i < _activeSteps.Count; i++)
        {
            if (_scenario.CurrentStepGroup.steps[i].id == step.id)
            {
                _activeSteps[i].SetState(step.stepState);
                break;
            }
        }

        if (_stepCoroutine != null)
        {
            StopCoroutine(_stepCoroutine);
        }

        _stepCoroutine = StartCoroutine(AnimateStepTransition());
    }


    private void HandleGroupCompleted(StepGroup group)
    {
        _groupInitialized = false;

        if (_stepCoroutine != null)
        {
            StopCoroutine(_stepCoroutine);
        }

        if (_groupCoroutine != null)
        {
            StopCoroutine(_groupCoroutine);
        }

        _groupCoroutine = StartCoroutine(AnimateGroupTransition());
    }

    
    // Для создания интерфейса группы
    private void SetupStepGroup(StepGroup group)
    {
        _groupNameText.text = group.name;

        foreach (var stepGO in _stepPool)
            stepGO.SetActive(false);

        _activeSteps.Clear();
        _stepStates.Clear();

        for (int i = 0; i < group.steps.Count; i++)
        {
            var step = group.steps[i];
            GameObject go = GetStepFromPool();
            go.transform.SetParent(_stepsRoot, false);
            go.SetActive(true);

            var stepUI = go.GetComponent<StepUI>();
            stepUI.SetState(step.stepState);

            _stepStates[step.id] = step.stepState;
            _activeSteps.Add(stepUI);
        }
    }

    private GameObject GetStepFromPool()
    {
        foreach (var stepGO in _stepPool)
        {
            if (!stepGO.activeSelf)
                return stepGO;
        }

        GameObject newStepGO = Instantiate(_stepPrefab);
        _stepPool.Add(newStepGO);
        return newStepGO;
    }
    
    private IEnumerator AnimateStepTransition()
    {
        yield return FadeOut(_descriptionCanvasGroup);

        yield return new WaitForSeconds(0.25f);
        
        if (_scenario.CurrentStep != null)
        {
            _stepDescriptionText.text = _scenario.CurrentStep.description;
        }

        yield return FadeIn(_descriptionCanvasGroup);
    }
    
    private IEnumerator AnimateGroupTransition()
    {
        _isTransitioningGroup = true;

        yield return new WaitForSeconds(1f);
    
        _stepDescriptionText.text = "Группа завершена!";

        yield return FadeIn(_descriptionCanvasGroup);
        yield return new WaitForSeconds(2f);

        yield return FadeOut(_canvasGroup);
    
        if (_currentGroupIndex < _positions.Count)
        {
            _helpUIGameObject.transform.position = _positions[_currentGroupIndex];
            _currentGroupIndex++;

            yield return new WaitForSeconds(_transitionDelay);
            yield return FadeIn(_canvasGroup);
        }
        else
        {
            _helpUIGameObject.SetActive(false);
        }

        _isTransitioningGroup = false;
        
        if (_deferredStepGroup != null)
        {
            SetupStepGroup(_deferredStepGroup);
            
            if (_scenario.CurrentStep != null && _scenario.CurrentStep.id == _deferredStepGroup.steps[0].id)
            {
                _stepDescriptionText.text = _deferredStepGroup.steps[0].description;
            }
            
            _groupInitialized = true;
            _deferredStepGroup = null;
        }
        
    }

    private IEnumerator FadeOut(CanvasGroup canvasGroup, float duration = 0.3f)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / duration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }

    private IEnumerator FadeIn(CanvasGroup canvasGroup, float duration = 0.3f)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / duration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

}
