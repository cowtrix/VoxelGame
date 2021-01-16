using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

public class VoxelManager : Singleton<VoxelManager>
{
	public Material DefaultMaterial;
	public Material DefaultMaterialDebug;
	public Material DefaultMaterialTransparent;

	public Texture2DArray TextureArray;
	public List<Texture2D> Sprites;

	[ContextMenu("Regenerate Spritesheet")]
	public void RegenerateSpritesheet()
	{
#if UNITY_EDITOR
		var newArray = Texture2DArrayGenerator.Generate(Sprites, TextureFormat.ARGB32);
		newArray.filterMode = FilterMode.Point;
		newArray.wrapMode = TextureWrapMode.Repeat;
		var currentPath = AssetDatabase.GetAssetPath(TextureArray);
		var tmpPath = AssetCreationHelper.CreateAssetInCurrentDirectory(newArray, "tmp.asset");
		File.WriteAllBytes(currentPath, File.ReadAllBytes(tmpPath));
		AssetDatabase.DeleteAsset(tmpPath);
		AssetDatabase.ImportAsset(currentPath);
		DefaultMaterial.SetTexture("AlbedoSpritesheet", TextureArray);
		DefaultMaterialTransparent.SetTexture("AlbedoSpritesheet", TextureArray);
#endif
	}
}
