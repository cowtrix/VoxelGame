using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Vector3 Rotation;

    void Update()
    {
        transform.localRotation *= Quaternion.Euler(Rotation * Time.deltaTime);  
    }
}
