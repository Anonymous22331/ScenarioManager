using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
// 
// Использовал для реализации "слежения" панелей за игроком
//
/// </summary>

public class LookTowards : MonoBehaviour
{
    [Header("Цель для отслеживания")]
    [SerializeField] private Transform target;

    [Header("Поворачиваться только по оси Y")]
    [SerializeField] private bool rotateWithY = false;

    [Header("Скорость поворота")]
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Минимальная дистанция отслеживания")]
    [SerializeField] private float minDistance = 0.3f;
    
    [Header("Смещение вращения")]
    [SerializeField] private Vector3 rotationOffset;

    private void Update()
    {
        if (target == null) return;

        Vector3 direction = target.position - transform.position;

        if (direction.magnitude < minDistance)
            return;

        Quaternion targetRotation;

        if (!rotateWithY)
        {
            Vector3 flatDirection = new Vector3(direction.x, 0f, direction.z);
            if (flatDirection.sqrMagnitude < 0.001f) return;
            targetRotation = Quaternion.LookRotation(flatDirection.normalized);
        }
        else
        {
            targetRotation = Quaternion.LookRotation(direction.normalized);
        }
        
        targetRotation *= Quaternion.Euler(rotationOffset);
        
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}
