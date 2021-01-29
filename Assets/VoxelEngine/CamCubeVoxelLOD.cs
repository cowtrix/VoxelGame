using Common;
using System;
using UnityEngine;

public class CamCubeVoxelLOD : VoxelLOD 
{
	[Range(sbyte.MinValue, sbyte.MaxValue)]
	public sbyte MinLayer = sbyte.MinValue;
	[Range(sbyte.MinValue, sbyte.MaxValue)]
	public sbyte MaxLayer = sbyte.MaxValue;

	public float Scale = 1;
	public int Resolution = 16;
	public Texture2DArray TexArray;

	private void OnWillRenderObject()
	{
		if(TexArray == null)
		{
			return;
		}
		if (m_propertyBlock == null)
		{
			m_propertyBlock = new MaterialPropertyBlock();
		}
		m_propertyBlock.SetTexture("Texture2DArray_735baf00590d4834930f7fb73661afe6", TexArray);
		MeshRenderer.SetPropertyBlock(m_propertyBlock);
	}

	public override void Rebuild(VoxelRenderer renderer)
	{
		var layer = renderer.gameObject.layer;
		var bounds = renderer.Bounds;
		MeshFilter.sharedMesh = VoxelManager.Instance.CubeMesh;
		var rot = renderer.transform.rotation;
		var up = renderer.transform.up;
		transform.localScale = bounds.size * Scale;
		transform.position = bounds.center;

		var maxLayer = renderer.MaxLayer;
		renderer.MaxLayer = MaxLayer;
		var minLayer = renderer.MinLayer;
		renderer.MinLayer = MinLayer;
		renderer.Invalidate(false);

		MeshRenderer.sharedMaterial = Resources.Load<Material>("VoxelEngine/LODMaterial");

		try
		{
			if (TexArray == null || TexArray.width != Resolution || TexArray.height != Resolution)
			{
				TexArray = new Texture2DArray(Resolution, Resolution, 6, TextureFormat.ARGB32, false);
			}

			/*if(Normal.x < 0){ Index = 0; }
			if(Normal.x > 0){ Index = 1; }

			if(Normal.y < 0){ Index = 2; }
			if(Normal.y > 0){ Index = 3; }

			if(Normal.z < 0){ Index = 4; }
			if(Normal.z > 0){ Index = 5; } */

			float bump = 1.5f;
			var left = GetTex(bounds.center + rot * Vector3.left * bounds.extents.x * Scale * bump,
				rot * Quaternion.LookRotation(Vector3.right, up),
				bounds.size.zyx() * Scale, Vector2.one * Resolution);
			TexArray.SetPixels(left.GetPixels(), 0);
			left.SafeDestroy();

			var right = GetTex(bounds.center + rot * Vector3.right * bounds.extents.x * Scale * bump,
				rot * Quaternion.LookRotation(Vector3.left, up),
				bounds.size.zyx() * Scale, Vector2.one * Resolution);
			TexArray.SetPixels(right.GetPixels(), 1);
			right.SafeDestroy();

			var bottom = GetTex(bounds.center + rot * Vector3.down * bounds.extents.y * Scale * bump,
				rot * Quaternion.LookRotation(Vector3.up, up),
				bounds.size.xzy() * Scale, Vector2.one * Resolution);
			TexArray.SetPixels(bottom.GetPixels(), 2);
			bottom.SafeDestroy();

			var top = GetTex(bounds.center + rot * Vector3.up * bounds.extents.y * Scale * bump,
				rot * Quaternion.LookRotation(Vector3.down, up),
				bounds.size.xzy() * Scale, Vector2.one * Resolution);
			TexArray.SetPixels(top.GetPixels(), 3);
			top.SafeDestroy();

			var back = GetTex(bounds.center + rot * Vector3.back * bounds.extents.x * Scale * bump,
				rot * Quaternion.LookRotation(Vector3.forward, -up),
				bounds.size * Scale, Vector2.one * Resolution);
			TexArray.SetPixels(back.GetPixels(), 4);
			back.SafeDestroy();

			var front = GetTex(bounds.center + rot * Vector3.forward * bounds.extents.x * Scale * bump,
				rot * Quaternion.LookRotation(Vector3.back, up),
				bounds.size * Scale, Vector2.one * Resolution);
			TexArray.SetPixels(front.GetPixels(), 5);
			front.SafeDestroy();

			TexArray.Apply();
		}
		catch (Exception) { throw; }
		finally
		{
			renderer.gameObject.layer = layer;
			renderer.MaxLayer = maxLayer;
			renderer.MinLayer = minLayer;
			renderer.Invalidate(false);
		}
	}
}
