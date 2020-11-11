using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public float RotateSpeed = 1;
    public CameraController CameraController => CameraController.Instance;
    public Transform Head;

    void Update()
    {
        if(CameraController.IsFreeLook)
		{
            return;
		}
        var lookRot = Quaternion.LookRotation((CameraController.FocusPoint - Head.transform.position).normalized, transform.up);
        Head.transform.rotation = Quaternion.Lerp(Head.transform.rotation, lookRot, RotateSpeed * Time.deltaTime);
    }
}
