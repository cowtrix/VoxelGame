using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Texture2DArrayGenerator
{
	const int SIZE = 32;

	static Texture2D Resize(Texture2D texture2D, int targetX, int targetY)
	{
		RenderTexture rt = new RenderTexture(targetX, targetY, 24);
		RenderTexture.active = rt;
		Graphics.Blit(texture2D, rt);
		Texture2D result = new Texture2D(targetX, targetY);
		result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
		result.Apply();
		return result;
	}

	public static Texture2DArray Generate(IList<Texture2D> textures, TextureFormat format)
	{
		var texture2DArray = new Texture2DArray(SIZE, SIZE, textures.Count, format, false);
		for (int i = 0; i < textures.Count; i++)
		{
			var tex = textures[i];
			if (tex.height != SIZE || tex.width != SIZE)
			{
				tex = Resize(tex, SIZE, SIZE);
			}
			texture2DArray.SetPixels(tex.GetPixels(), i);
		}

		texture2DArray.Apply();

		return texture2DArray;
	}
}