using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Gauge : MonoBehaviour
{
    public Transform Hand;
    public Vector3 MinRotation, MaxRotation;
    [Range(0, 1)]
    public float Value;

    private void Update()
    {
        Hand.localRotation = Quaternion.Euler(Vector3.Lerp(MinRotation, MaxRotation, Value));
    }
}
