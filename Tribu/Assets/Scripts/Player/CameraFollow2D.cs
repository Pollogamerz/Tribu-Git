using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CameraFollow2D : MonoBehaviour
{
    private Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    void Start()
    {
        if (target != null)
        {
            Vector3 initialPosition = target.position + offset;
            initialPosition.z = transform.position.z;
            transform.position = initialPosition;
        }
        foreach (var player in FindObjectsOfType<PlayerController>())
        {
            if (player.IsOwner)
            {
                target = player.transform;
                break;
            }
        }
    }

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            desiredPosition.z = transform.position.z;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}
