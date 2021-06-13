using Common;
using UnityEngine;
using Voxul;

[ExecuteAlways]
public class VoxelTrail : MonoBehaviour {
	VoxelRenderer Renderer => GetComponent<VoxelRenderer>();

	Vector3 m_lastPos;
	Quaternion m_lastRot;



	private void Update()
	{
		if(transform.position == m_lastPos && transform.rotation == m_lastRot)
		{
			return;
		}
		foreach(var vox in Renderer.Mesh.Voxels.Keys)
		{
			var pos = Renderer.transform.localToWorldMatrix.MultiplyPoint3x4(vox.ToVector3());
			var scale = VoxelCoordinate.LayerToScale(vox.Layer);
			var col = Color.white;

			DebugHelper.DrawCube(pos, Vector3.one * scale * .5f, transform.rotation, col.WithAlpha(.1f), 20_000);
		}
		m_lastPos = transform.position;
		m_lastRot = transform.rotation; 
	}
}
