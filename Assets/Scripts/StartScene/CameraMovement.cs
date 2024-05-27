using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    [SerializeField] private Transform center;
    [SerializeField] private Transform cameraLookAt;
    private float moveSpeedAngle = Mathf.PI / 12.0f;
    float angle = Mathf.PI / 2.0f;
    float radius;



    void Start()
    {
        radius = (center.position - transform.position).magnitude;
    }

    void Update()
    {

        float x = center.position.x + Mathf.Cos(angle) * radius;
        float z = center.position.z - Mathf.Sin(angle) * radius;

        transform.position = new Vector3(x, center.position.y, z);

        Quaternion rotation = Quaternion.LookRotation(cameraLookAt.position - transform.position, Vector3.up);
        transform.rotation = rotation;

        angle += moveSpeedAngle * Time.deltaTime;
    }
}
