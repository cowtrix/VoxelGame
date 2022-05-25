using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxul;

[RequireComponent(typeof(VoxelRenderer))]
public class NeonSign : RenderBehaviour
{
	[GradientUsage(true)]
    public Gradient FlickerGradient;
	private Color m_color;

    protected VoxelColorTint Tint { get; private set; }

    protected override void UpdateOnScreen()
    {
        if (!Tint)
        {
            Tint = GetComponent<VoxelColorTint>();
        }
        Tint.Color = FlickerGradient.Evaluate(Random.value);
        Tint.Invalidate();
    }
}
