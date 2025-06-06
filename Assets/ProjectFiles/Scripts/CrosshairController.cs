using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

/// <summary>
// 
// Контроллер прицела для ПК
//
/// </summary>

public class CrosshairController : MonoBehaviour
{
    [SerializeField] private GameObject _crosshairUI;
    [SerializeField] private float _checkInterval = 1f;

    private bool _lastIsVR;
    private float _timer;

    void Start()
    {
        UpdateCrosshairState();
    }

    void Update()
    {
        _timer += Time.unscaledDeltaTime;
        if (_timer >= _checkInterval)
        {
            _timer = 0f;
            UpdateCrosshairState();
        }
    }
    
    private void UpdateCrosshairState()
    {
        bool isVR = XRSettings.isDeviceActive;

        if (isVR != _lastIsVR)
        {
            _crosshairUI.SetActive(!isVR);
            _lastIsVR = isVR;
        }
    }
}
