using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
// 
// Контроллер панели в лобби
//
/// </summary>


public class ScenarioPanelController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private ScenarioLibrary _scenarioLibrary;
    
    [Header("UI")]
    [SerializeField] private Transform _buttonContainer;
    [SerializeField] private GameObject _scenarioButtonPrefab;

    private void Start()
    {
        CreateScenarioButtons();
    }

    private void CreateScenarioButtons()
    {
        foreach (var scenario in _scenarioLibrary.scenarios)
        {
            var button = Instantiate(_scenarioButtonPrefab, _buttonContainer);
            var buttonText = button.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
                buttonText.text = scenario.DisplayName;

            var scenarioId = scenario.Id;
            button.GetComponent<Button>().onClick.AddListener(() => OnScenarioSelected(scenarioId));
        }
    }

    private void OnScenarioSelected(string scenarioId)
    {
        ScenarioContext.SelectedScenarioId = scenarioId;

        var scenario = _scenarioLibrary.GetById(scenarioId);
        if (scenario.HasValue)
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scenario.Value.SceneToLoad);
        }
        else
        {
            Debug.LogError($"Сценарий с id {scenarioId} не найден.");
        }
    }
}

public static class ScenarioContext
{
    public static string SelectedScenarioId { get; set; }
}