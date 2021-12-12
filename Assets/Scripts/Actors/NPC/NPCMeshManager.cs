using Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul;
using Voxul.Meshing;

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

public class NPCMeshManager : MonoBehaviour
{
	public int Seed;
	public TriColorSet Colors;

	protected IEnumerable<NPCMeshSegment> Children => GetComponentsInChildren<NPCMeshSegment>();

	private void Reset()
	{
		Seed = Random.Range(int.MinValue, int.MaxValue);
	}

	[ContextMenu("Generate")]
	public void Generate()
	{
		Random.InitState(Seed);
		Colors.BaseColor = Random.ColorHSV(0, 1, 1, 1, 1, 1);
		foreach (var r in Children)
		{
			if (r.Collection && r.Collection.Data.Any())
			{
				r.Renderer.Mesh = r.Collection.GetWeightedRandom<VoxelMesh>();
				var color = r.GetComponent<VoxelColorTint>();
				if (color)
				{
					color.Color = Colors.GetColor(r.ColorMode)
						.Saturate(r.Saturation);
					color.Invalidate();
				}
			}
		}
	}
}
