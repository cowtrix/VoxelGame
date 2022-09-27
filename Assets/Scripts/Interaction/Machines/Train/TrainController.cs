using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxul;
using Voxul.Utilities;
using vSplines;

public class TrainController : ExtendedMonoBehaviour
{
    public int CurrentDirection { get; private set; } = 1;
    public float CurrentDistance { get; private set; } = 30;

    public float Speed = 1;
    public bool Slave;
    public Trainline Trainline;
    public Vector3 Rotation;
    public Vector2 Buffer;
    public float Offset = 1;

    public List<TrainController> Carriages;
    public float Space = .1f;

    private Rigidbody Rigidbody => GetComponent<Rigidbody>();

    private void Start()
    {
        if (!Slave)
        {
            StartCoroutine(Drive());
        }
    }

    private IEnumerator Drive()
    {
        var nextStopIndex = 0;
        while (true)
        {
            yield return null;
            SetPosition();
            if (nextStopIndex >= Trainline.Line.Count)
            {
                nextStopIndex -= 2;
                CurrentDirection = -1;
                Debug.Log("Train hit end of line, turning around");
                continue;
            }
            else if (nextStopIndex < 0)
            {
                nextStopIndex = 1;
                CurrentDirection = 1;
                Debug.Log("Train hit start of line, turning around");
                continue;
            }

            var nextStop = Trainline.Line[nextStopIndex];
            DebugHelper.DrawCube(nextStop.transform.position, Vector3.one * 5, Quaternion.identity, Color.magenta, 0);
            CurrentDistance += Speed * Time.deltaTime * CurrentDirection;
            var buffer = CurrentDirection == 1 ? Buffer.x : Buffer.y;
            if ((CurrentDirection == 1 && CurrentDistance >= nextStop.Distance + buffer) ||
                (CurrentDirection == -1 && Trainline.Track.Length - CurrentDistance >= Trainline.Track.Length - (nextStop.Distance + buffer)))
            {
                if (nextStop.StopTime > 0)
                {
                    Debug.Log($"Stopping at station {nextStop} for {nextStop.StopTime}s", nextStop);
                    yield return new WaitForSeconds(nextStop.StopTime);
                }
                nextStopIndex += CurrentDirection;
            }
        }
    }

    public void SetPosition()
    {
        var upOffset = Trainline.Track.GetDistanceUpAlongSpline(CurrentDistance) * Offset;
        var p = Trainline.Track.GetDistancePointAlongSpline(CurrentDistance) + upOffset;
        var n = Trainline.Track.GetDistancePointAlongSpline(CurrentDistance + 1) + upOffset;
        Rigidbody.MovePosition(p);
        Rigidbody.MoveRotation(Quaternion.LookRotation((p - n).normalized) * Quaternion.Euler(Rotation * CurrentDirection * -1));
        for (int i = 0; i < Carriages.Count; i++)
        {
            var carriage = Carriages[i];
            if (!carriage)
            {
                continue;
            }
            carriage.CurrentDistance = CurrentDistance - (i + 1) * Space;
            carriage.SetPosition();
        }
    }
}
