using Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul;
using Voxul.Meshing;
using Voxul.Utilities;

[System.Serializable]
public class TriColorSet
{
	public enum eColorMode
	{
		Base, Compliment1, Compliment2
	}

	public Color BaseColor;
	public Color Compliment1 => HueShift(BaseColor, 1 / 3f);
	public Color Compliment2 => HueShift(BaseColor, -1 / 3f);

	private static Color HueShift(Color c, float amount)
	{
		Color.RGBToHSV(c, out var h, out var s, out var v);
		h += amount;
		while (h > 1)
			h--;
		while (h < 0)
			h++;
		return Color.HSVToRGB(h, s, v);
	}

	public Color GetColor(eColorMode mode)
	{
		switch (mode)
		{
			case eColorMode.Base:
				return BaseColor;
			case eColorMode.Compliment1:
				return Compliment1;
			case eColorMode.Compliment2:
				return Compliment2;
		}
		return BaseColor;
	}
}

public class NPCMeshManager : ExtendedMonoBehaviour
{
	public int Seed;
	public TriColorSet Colors;

	protected IEnumerable<NPCMeshSegment> Children => GetComponentsInChildren<NPCMeshSegment>();

	private void Reset()
	{
		Seed = Random.Range(int.MinValue, int.MaxValue);
	}

	private void Start()
	{
		foreach(var c in Children)
		{
			c.GetComponent<VoxelColorTint>()?.Invalidate();
		}
	}

	[ContextMenu("Generate")]
	public void Generate()
	{
		Random.InitState(Seed);
		Colors.BaseColor = Random.ColorHSV(0, 1, 1, 1, 1, 1);
		var scaleFactor = Random.value;
		foreach (var r in Children)
		{
			// Pick random mesh
			if (r.Collection && r.Collection.Data.Any() && r.Renderer)
			{
				r.Renderer.Mesh = r.Collection.GetWeightedRandom<VoxelMesh>();
				r.Renderer.Invalidate(true, false);
			}

			// Set color tint
			var color = r.GetComponent<VoxelColorTint>();
			if (color)
			{
				color.Color = Colors.GetColor(r.ColorMode)
					.Saturate(r.Saturation);
				color.Invalidate();
				color.TrySetDirty();
			}

			// Random scale
			r.transform.localScale = Vector3.one * Mathf.Lerp(r.Scale.x, r.Scale.y, scaleFactor);
			r.gameObject.TrySetDirty();
		}
		foreach(var line in GetComponentsInChildren<BezierConnectorLineRenderer>(true))
		{
			line.Invalidate();
		}
	}
}
