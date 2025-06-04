using UnityEngine;
using UnityEngine.UI;

public class ScenarioPanelController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private ScenarioLibrary scenarioLibrary;

    [Header("UI")]
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private Button scenarioButtonPrefab;

    private void Start()
    {
        CreateScenarioButtons();
    }

    private void CreateScenarioButtons()
    {
        foreach (var scenario in scenarioLibrary.scenarios)
        {
            var button = Instantiate(scenarioButtonPrefab, buttonContainer);
            var buttonText = button.GetComponentInChildren<Text>();
            if (buttonText != null)
                buttonText.text = scenario.DisplayName;

            var scenarioId = scenario.Id;
            button.onClick.AddListener(() => OnScenarioSelected(scenarioId));
        }
    }

    private void OnScenarioSelected(string scenarioId)
    {
        ScenarioContext.SelectedScenarioId = scenarioId;

        var scenario = scenarioLibrary.GetById(scenarioId);
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