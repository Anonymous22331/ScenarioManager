using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
// 
// SO для хранения всех сценариев
//
/// </summary>

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

[Serializable]
public struct ScenarioDefinition
{
    public string Id;
    public string DisplayName;
    public string SceneToLoad;
}