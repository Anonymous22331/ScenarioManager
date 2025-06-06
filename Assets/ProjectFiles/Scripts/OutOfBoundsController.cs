using System.Collections.Generic;
using UnityEngine;

/// <summary>
// 
// Если вдруг игрок выкинет важный предмет или сам выпадет за карту
//
/// </summary>

public class OutOfBoundsController : MonoBehaviour
{
    public string[] allowedTags = new string[] { "Player", "Grabbable" };
    
    private Dictionary<Transform, Vector3> _startPositions = new Dictionary<Transform, Vector3>();
    private Dictionary<Transform, Quaternion> _startRotations = new Dictionary<Transform, Quaternion>();

    private void Start()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (var obj in allObjects)
        {
            foreach (var tag in allowedTags)
            {
                if (obj.CompareTag(tag))
                {
                    Transform root = obj.transform;
                    if (!_startPositions.ContainsKey(root))
                    {
                        _startPositions[root] = root.position;
                        _startRotations[root] = root.rotation;
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform objectTransform = other.transform;
        Debug.Log($"{objectTransform.name} and {other.name}");
        foreach (var tag in allowedTags)
        {
            if (objectTransform.CompareTag(tag))
            {
                if (!_startPositions.ContainsKey(objectTransform))
                {
                    _startPositions[objectTransform] = objectTransform.position;
                    _startRotations[objectTransform] = objectTransform.rotation;
                }

                ReturnOnStartPosition(objectTransform.gameObject);
                return;
            }
        }
    }

    private void ReturnOnStartPosition(GameObject obj)
    {
        Transform objectTransform = obj.transform;

        if (_startPositions.TryGetValue(objectTransform, out Vector3 pos) && _startRotations.TryGetValue(objectTransform, out Quaternion rot))
        {
            Rigidbody rb = objectTransform.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            objectTransform.position = pos;
            objectTransform.rotation = rot;

            Debug.Log($"{objectTransform.name} возвращён на стартовую позицию.");
        }
        else
        {
            Debug.LogWarning($"{objectTransform.name} не имеет записанной стартовой позиции.");
        }
    }
}
