using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

/// <summary>
// 
// Управление клава/мышь на основе CharacterController 
// Не конфликтует с VR
//
/// </summary>

[RequireComponent(typeof(CharacterController))]
public class XRPCController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 2.0f;
    [SerializeField] private float _mouseLookSensitivity = 2.0f;

    private CharacterController _characterController;
    private XROrigin _xrOrigin;

    private float _rotationX = 0f;
    private float _rotationY = 0f;

    private Transform _cameraTransform;
    
    private bool _isXRActive;
    
    private float _checkInterval = 4f;
    private float _timeSinceLastCheck = 0f;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _xrOrigin = GetComponent<XROrigin>();
    }

    private void Start()
    {
        _cameraTransform = _xrOrigin.Camera.transform;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        HandleMovement();
        
        _timeSinceLastCheck += Time.deltaTime;
        if (_timeSinceLastCheck >= _checkInterval)
        {
            UpdateHmdState();
            _timeSinceLastCheck = 0f;
        }
        
        if (!_isXRActive)
        {
            HandleMouseLook();
        }
    }

    private void HandleMovement()
    {
        Vector3 input = new Vector3(
            Keyboard.current.dKey.isPressed ? 1 : Keyboard.current.aKey.isPressed ? -1 : 0,
            0,
            Keyboard.current.wKey.isPressed ? 1 : Keyboard.current.sKey.isPressed ? -1 : 0
        );

        if (input.sqrMagnitude < 0.01f)
            return;

        input.Normalize();
        Vector3 move = _cameraTransform.TransformDirection(input);
        move.y = 0; // no vertical movement
        _characterController.Move(move * _moveSpeed * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * _mouseLookSensitivity * Time.deltaTime;

        _rotationX -= mouseDelta.y;
        _rotationY += mouseDelta.x;

        _rotationX = Mathf.Clamp(_rotationX, -80f, 80f);
        
        _xrOrigin.CameraFloorOffsetObject.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
        transform.rotation = Quaternion.Euler(0, _rotationY, 0);
    }
    
    private void UpdateHmdState()
    {
        bool hmdActiveNow = XRSettings.isDeviceActive;

        if (hmdActiveNow == _isXRActive)
            return;

        _isXRActive = hmdActiveNow;
        Cursor.lockState = _isXRActive
            ? CursorLockMode.None
            : CursorLockMode.Locked;
    }
    
}
