using Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul;
using Voxul.Utilities;

public class TintGradient : ExtendedMonoBehaviour
{
    public Gradient YGradient, FlatGradient;
    public float Jitter;

    [ContextMenu("Invalidate")]
    public void Invalidate()
    {
        var tints = GetComponentsInChildren<VoxelColorTint>();
        var bounds = tints
            .SelectMany(t => t.Renderers.Select(r => r.Bounds))
            .GetEncompassingBounds();
        for (int i = 0; i < tints.Length; i++)
        {
            var tint = tints[i];
            var relativePos = tint.transform.position - bounds.min;
            Random.InitState(tint.GetHashCode());
            relativePos += new Vector3(Random.Range(-Jitter, Jitter), Random.Range(-Jitter, Jitter), Random.Range(-Jitter, Jitter));
            var normalizedPos = new Vector3(relativePos.x / bounds.size.x, relativePos.y / bounds.size.y, relativePos.z / bounds.size.z);
            Debug.Log(normalizedPos);
            tint.Color = FlatGradient.Evaluate(normalizedPos.x) + YGradient.Evaluate(normalizedPos.y) + FlatGradient.Evaluate(normalizedPos.z);
            tint.RefreshRenderers();
            tint.Invalidate();
        }
    }
}
