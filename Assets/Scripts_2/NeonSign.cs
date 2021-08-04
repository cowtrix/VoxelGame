using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxul;

[RequireComponent(typeof(VoxelRenderer))]
[RequireComponent(typeof(VoxelColorTint))]
public class NeonSign : MonoBehaviour
{
	[GradientUsage(true)]
    public Gradient FlickerGradient;

    protected VoxelColorTint Tint => GetComponent<VoxelColorTint>();

	private void Update()
	{
		Tint.Color = FlickerGradient.Evaluate(Random.value);
		Tint.Invalidate();
	}
}
