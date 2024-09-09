using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothness = 0.1f;
    public Vector3 offset;

    void LateUpdate()
    {
        Vector3 targetPos = target.position + offset;
        Vector3 smoothedPos = Vector3.Lerp(transform.position, targetPos, smoothness * Time.deltaTime);

        transform.position = smoothedPos;
    }
}
