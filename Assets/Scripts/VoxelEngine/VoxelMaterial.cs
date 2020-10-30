using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ENormalMode
{
	Hard,
}

public enum EVoxelDirection
{
	YNeg, XNeg,
	ZPos, ZNeg,
	XPos, YPos,
}

[Serializable]
public struct DirectionOverride
{
	public EVoxelDirection Direction;
	public SurfaceData Data;
}

[Serializable]
public struct SurfaceData
{
	[ColorUsage(false, true)]
	public Color Albedo;
	[Range(0, 1)]
	public float Metallic;
	[Range(0, 1)]
	public float Smoothness;
	public int TextureIndex;
}

[Serializable]
public struct VoxelMaterialData
{
	public ENormalMode NormalMode;
	public SurfaceData Default;
	public DirectionOverride[] Overrides;

	public SurfaceData GetSurface(EVoxelDirection dir)
	{
		var ov = Overrides.Where(o => o.Direction == dir);
		if (ov.Any())
		{
			return ov.Single().Data;
		}
		return Default;
	}

	public VoxelMaterialData Copy()
	{
		return new VoxelMaterialData
		{ 
			Default = Default, 
			Overrides = Overrides?.ToArray() 
		};
	}
}

[CreateAssetMenu]
public class VoxelMaterial : ScriptableObject
{
	public VoxelMaterialData Data;
}
