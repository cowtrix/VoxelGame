using Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul;
using Voxul.Utilities;

public class CameraMagnetFunction : NodeCanvas.Tasks.Actions.ExecuteFunction
{
    protected override void OnExecute()
    {

        base.OnExecute();
    }
}

public class CameraMagnet : TrackedObject<CameraMagnet>
{
    public CameraController Camera => CameraController.Instance;
    public float NormalizedTime { get; private set; }

    public Vector3 AdditionalRot = new Vector3(0, -90, 0);
    public string Label;
    public bool PlayOnStart = false;
    public float Duration = 1;
    public float Strength = 1;
    public AnimationCurve StrengthCurve = AnimationCurve.Linear(0, 1, 1, 1);

    void Start()
    {
        //Camera = CameraController.Instance;
        if (!PlayOnStart)
        {
            enabled = false;
        }
    }

    [ContextMenu("Play")]
    public void PlayCameraMagnet()
    {
        Debug.Log($"Playing Camer Magnet {Label}");
        NormalizedTime = 0;
        enabled = true;
    }

    void Update()
    {
        var dt = Time.deltaTime / Duration;
        NormalizedTime = Mathf.Clamp01(dt + NormalizedTime);
        if(NormalizedTime >= 1)
        {
            enabled = false;
            return;
        }

        var lookStrength = Strength * StrengthCurve.Evaluate(NormalizedTime) * Time.deltaTime;

        Debug.DrawLine(Camera.transform.position, transform.position, Color.green);
        var targetDelta = transform.position - Camera.transform.position;
        var newForward = Quaternion.Euler(AdditionalRot) * Vector3.RotateTowards(Camera.transform.forward, targetDelta.normalized, lookStrength, lookStrength);

        Debug.DrawLine(Camera.transform.position, Camera.transform.position + newForward, Color.magenta);
        Common.DebugHelper.DrawPoint(Camera.transform.position + newForward, .1f, Color.magenta, 0);
        Camera.LookAt(newForward);
    }
}
