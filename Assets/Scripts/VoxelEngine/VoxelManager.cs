using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VoxelManager : Singleton<VoxelManager>
{
	public Material DefaultMaterial;
	public Texture2DArray TextureArray;
	public List<Texture2D> Sprites;

	[ContextMenu("Regenerate Spritesheet")]
	public void RegenerateSpritesheet()
	{
		Texture2DArrayGenerator.Generate(TextureArray, Sprites, TextureFormat.ARGB32);
	}
}
