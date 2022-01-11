using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxul;

[RequireComponent(typeof(VoxelRenderer))]
public class NeonSign : VoxelRendererPropertyModifier
{
	[GradientUsage(true)]
    public Gradient FlickerGradient;

	private Color m_color;

    protected VoxelColorTint Tint => GetComponent<VoxelColorTint>();

	private void Update()
	{
		m_color = FlickerGradient.Evaluate(Random.value);
		Invalidate();
	}

	protected override void SetPropertyBlock(MaterialPropertyBlock block, VoxelRendererSubmesh submesh)
	{
		block.SetColor("AlbedoTint", m_color);
	}
}
