using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EUVMode : byte
{
	Local, LocalScaled, Global, GlobalScaled,
}

public enum ENormalMode : byte
{
	Hard,
}

public enum EVoxelDirection : byte
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
public struct TextureIndex
{
	public int Index;
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
	public TextureIndex Texture;
	public EUVMode UVMode;
}

[Serializable]
public struct VoxelMaterial
{
	public ENormalMode NormalMode;
	public SurfaceData Default;
	public DirectionOverride[] Overrides;

	public SurfaceData GetSurface(EVoxelDirection dir)
	{
		if (Overrides != null)
		{
			var ov = Overrides.Where(o => o.Direction == dir);
			if (ov.Any())
			{
				return ov.Single().Data;
			}
		}

		return Default;
	}

	public VoxelMaterial Copy()
	{
		return new VoxelMaterial
		{
			Default = Default,
			Overrides = Overrides?.ToArray()
		};
	}
}

[CreateAssetMenu]
public class VoxelMaterialAsset : ScriptableObject
{
	public VoxelMaterial Data;
}
