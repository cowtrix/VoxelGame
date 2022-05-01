using System.Collections.Generic;
using UnityEngine;
using Voxul;

[ExecuteAlways]
public class TrainController : ExtendedMonoBehaviour
{
    public float Speed = 1;
    public float Time { get; private set; }
    public Trainline Trainline;
    public Vector3 Rotation;

    public List<TrainController> Carriages;
    public float Space = .1f;

    private Rigidbody Rigidbody => GetComponent<Rigidbody>();

    private void Update()
    {
        var p = Trainline.GetPointOnLine(Time);
        var n = Trainline.GetPointOnLine(Time + .01f);
        Rigidbody.MovePosition(p);
        Rigidbody.MoveRotation(Quaternion.LookRotation((transform.position - n).normalized) * Quaternion.Euler(Rotation));
        for (int i = 0; i < Carriages.Count; i++)
        {
            var carriage = Carriages[i];
            if (!carriage)
            {
                continue;
            }
            carriage.Time = Time - (i) * Space;
        }
    }
}
