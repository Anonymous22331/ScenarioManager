using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
// 
// UI контроллер для финальной панели
// 
/// </summary>


public class ScenarioResultUIController : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private ScenarioController _scenario;

    [SerializeField] private ScenarioList _scenarioList;
    [SerializeField] private GameObject _resultPanel;

    [SerializeField] private Transform _groupResultRoot;
    [SerializeField] private GameObject _groupResultPrefab;
    [SerializeField] private GameObject _stepResultPrefab;

    [Header("Buttons")]
    [SerializeField] private Transform _buttonsRoot;
    [SerializeField] private GameObject _buttonPrefab;
    [SerializeField] private string _lobbySceneName = "LobbyScene";

    private List<GameObject> _poolGroups = new();
    private List<GameObject> _poolSteps = new();
    private List<GameObject> _poolButtons = new();

    private void OnEnable() => _scenario.OnScenarioCompleted += BuildResultUI;
    private void OnDisable() => _scenario.OnScenarioCompleted -= BuildResultUI;

    private void BuildResultUI()
    {
        _resultPanel.SetActive(true);
        DeactivateAll(_poolGroups);
        DeactivateAll(_poolSteps);
        DeactivateAll(_poolButtons);

        foreach (var group in _scenarioList.groups)
        {
            GameObject groupGO = GetOrCreate(_groupResultPrefab, _groupResultRoot, _poolGroups);
            groupGO.transform.Find("GroupName").GetComponent<TextMeshProUGUI>().text = group.name;

            Transform stepsRoot = groupGO.transform.Find("StepsContainer");

            foreach (var step in group.steps)
            {
                GameObject stepGO = GetOrCreate(_stepResultPrefab, stepsRoot, _poolSteps);

                var textId = stepGO.transform.Find("TextID").GetComponent<TextMeshProUGUI>();
                var textRes = stepGO.transform.Find("StepResult").GetComponent<TextMeshProUGUI>();
                var stepUI = stepGO.transform.Find("Step").GetComponent<StepUI>();

                textId.text = $"Шаг {step.id}";
                bool success = step.stepState == StepState.Completed;

                textRes.text = success ? "Успешно" : "Не успешно";
                textRes.color = success ? Color.green : Color.red;

                stepUI.SetState(step.stepState);
            }
        }

        CreateBottomButtons();
        ForceUpdateFinishCanvas();
    }

    // Принудительно обновляет canvas, чтобы всё корректно отобразилось после создания элементов.
    private void ForceUpdateFinishCanvas()
    {
        Canvas.ForceUpdateCanvases();
        RectTransform rt = _groupResultRoot.GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
    }

    private void CreateBottomButtons()
    {
        GameObject lobbyBtn = GetOrCreate(_buttonPrefab, _buttonsRoot, _poolButtons);
        SetupButton(lobbyBtn, "В лобби", () => SceneManager.LoadSceneAsync(_lobbySceneName));

        GameObject restartBtn = GetOrCreate(_buttonPrefab, _buttonsRoot, _poolButtons);
        SetupButton(restartBtn, "Заново", () => SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex));
    }

    private static void SetupButton(GameObject uiButtonObject, string buttonText, UnityEngine.Events.UnityAction action)
    {
        var buttonObject = uiButtonObject.GetComponent<Button>();
        var text = uiButtonObject.GetComponentInChildren<TextMeshProUGUI>();

        text.text = buttonText;
        buttonObject.onClick.RemoveAllListeners();
        buttonObject.onClick.AddListener(action);
    }

    private static void DeactivateAll(IEnumerable<GameObject> list)
    {
        foreach (var go in list) go.SetActive(false);
    }

    private static GameObject GetOrCreate(GameObject prefab, Transform parent, List<GameObject> pool)
    {
        foreach (var go in pool)
        {
            if (!go.activeSelf)
            {
                go.transform.SetParent(parent, false);
                go.SetActive(true);
                return go;
            }
        }

        GameObject inst = Instantiate(prefab, parent);
        pool.Add(inst);
        return inst;
    }
}