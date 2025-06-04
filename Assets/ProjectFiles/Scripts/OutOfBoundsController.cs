using System.Collections.Generic;
using UnityEngine;

public class OutOfBoundsController : MonoBehaviour
{
     public string[] allowedTags = new string[] { "Player", "Grabbable" };
     
    private Dictionary<Transform, Vector3> startPositions = new Dictionary<Transform, Vector3>();
    private Dictionary<Transform, Quaternion> startRotations = new Dictionary<Transform, Quaternion>();

    private void Start()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (var obj in allObjects)
        {
            foreach (var tag in allowedTags)
            {
                if (obj.CompareTag(tag))
                {
                    Transform root = obj.transform.root;
                    if (!startPositions.ContainsKey(root))
                    {
                        startPositions[root] = root.position;
                        startRotations[root] = root.rotation;
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform objectTransform = other.transform.root;

        foreach (var tag in allowedTags)
        {
            if (objectTransform.CompareTag(tag))
            {
                if (!startPositions.ContainsKey(objectTransform))
                {
                    startPositions[objectTransform] = objectTransform.position;
                    startRotations[objectTransform] = objectTransform.rotation;
                }

                ReturnOnStartPosition(objectTransform.gameObject);
                return;
            }
        }
    }

    private void ReturnOnStartPosition(GameObject obj)
    {
        Transform objectTransform = obj.transform.root;

        if (startPositions.TryGetValue(objectTransform, out Vector3 pos) && startRotations.TryGetValue(objectTransform, out Quaternion rot))
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
