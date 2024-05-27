using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingTrap : MonoBehaviour
{
    [SerializeField] private Transform rotationCenter;
    [SerializeField] private float rotationSpeed = 50f;

    void Update()
    {
        if (rotationCenter != null)
        {
            transform.RotateAround(rotationCenter.position, Vector3.up, rotationSpeed * Time.deltaTime);
        }
        else
        {
            Debug.LogWarning("Rotation center is not set. Make sure to assign it in the inspector.");
        }
    }
}
