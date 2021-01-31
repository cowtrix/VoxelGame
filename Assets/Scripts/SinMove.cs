using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinMove : MonoBehaviour
{
    public float Frequency = 1;
    public Vector3 Offset = Vector3.up;

    private Vector3 m_pos;

    void Start()
    {
        m_pos = transform.localPosition;
    }

    void Update()
    {
        transform.localPosition = m_pos + Mathf.Sin(Time.time * Frequency) * Offset;
    }
}
