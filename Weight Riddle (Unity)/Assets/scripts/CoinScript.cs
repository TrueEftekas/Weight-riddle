using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Awake()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    // call this function to reset this script's object's position and rotation
    public void resetTransform()
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
    }
}
