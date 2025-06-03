using System.Collections.Generic;
using UnityEngine;

// SO для хранения всех сценариев
// 

[CreateAssetMenu(fileName = "ScenarioLibrary", menuName = "Scenarios/Scenario Library")]
public class ScenarioLibrary : ScriptableObject
{
    public List<ScenarioDefinition> scenarios;

    public ScenarioDefinition? GetById(string id)
    {
        foreach (var scenario in scenarios)
        {
            if (scenario.Id == id)
                return scenario;
        }

        return null;
    }
}