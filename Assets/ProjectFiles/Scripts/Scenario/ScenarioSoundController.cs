using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
// 
// Контроллер звуков сценария
//
/// </summary>


public class ScenarioSoundController : MonoBehaviour
{
    [Header("Scenario reference")]
    [SerializeField] private ScenarioController scenarioController;
    [Header("Audio")]
    [SerializeField] private AudioClip correctClip;
    [SerializeField] private AudioClip incorrectClip;
    [Range(0f, 1f)] [SerializeField] private float volume = 0.5f;

    private AudioSource _source;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        _source.playOnAwake = false;
    }

    private void OnEnable()
    {
        if (scenarioController != null)
            scenarioController.OnStepCompleted += HandleStepCompleted;
        else
            Debug.LogError("[ScenarioSoundController] ScenarioController не присоединен!");
    }

    private void OnDisable()
    {
        if (scenarioController != null)
            scenarioController.OnStepCompleted -= HandleStepCompleted;
    }
    
    private void HandleStepCompleted(Step step)
    {
        if (step == null) return;

        switch (step.stepState)
        {
            case StepState.Completed:
                PlayOneShot(correctClip);
                break;

            case StepState.Failed:
                PlayOneShot(incorrectClip);
                break;
            
            default:
                Debug.LogWarning($"[ScenarioSoundController] Неизвестное состояние {step.stepState} для шага {step.id}");
                break;
        }
    }

    private void PlayOneShot(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("[StepSoundController] Attempted to play a null AudioClip.");
            return;
        }

        _source.PlayOneShot(clip, volume);
    }

}