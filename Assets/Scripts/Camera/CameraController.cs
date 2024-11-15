using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Vector3 offset;

    [SerializeField]
    private Transform target;

    [SerializeField]
    private float minZoom = 1.0f;
    [SerializeField]
    private float maxZoom = 1.0f;

    private float zoom = 1;

    private void LateUpdate()
    {
        zoom = 1;
        transform.position = target.transform.position + (offset * zoom);
    }
}
