using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteAlways]
public class VoxelRenderer : MonoBehaviour
{
	public bool CustomMaterials;
	public bool GenerateCollider;
	public bool SnapToGrid;

	[Range(sbyte.MinValue, sbyte.MaxValue)]
	public sbyte MinLayer = sbyte.MinValue;
	[Range(sbyte.MinValue, sbyte.MaxValue)]
	public sbyte MaxLayer = sbyte.MaxValue;

	[Range(sbyte.MinValue, sbyte.MaxValue)]
	public sbyte SnapLayer = 0;

	public VoxelMesh Mesh;

	private MeshFilter m_filter;
	public MeshRenderer MeshRenderer;
	private MeshCollider m_collider;

	[SerializeField]
	[HideInInspector]
	private string m_lastMeshHash;

	public Bounds Bounds => MeshRenderer.bounds;

	private void Start()
	{
		Invalidate(false);
	}

	

	private void Update()
	{
		if(SnapToGrid)
		{
			var scale = VoxelCoordinate.LayerToScale(SnapLayer);
			transform.localPosition = transform.localPosition.RoundToIncrement(scale / (float)VoxelCoordinate.LayerRatio);
		}
		if (Mesh?.Hash == m_lastMeshHash)
		{
			return;
		}
		Invalidate(false);
	}

	private void SetupComponents(bool forceCollider)
	{
		if (!m_filter)
		{
			m_filter = gameObject.GetOrAddComponent<MeshFilter>();
		}
		//m_filter.hideFlags = HideFlags.HideAndDontSave;
		if (!MeshRenderer)
		{
			MeshRenderer = gameObject.GetOrAddComponent<MeshRenderer>();
		}
		//m_renderer.hideFlags = HideFlags.HideAndDontSave;
		if(GenerateCollider && !forceCollider)
		{
			if (!m_collider)
			{
				m_collider = gameObject.GetOrAddComponent<MeshCollider>();
			}
			//m_collider.hideFlags = HideFlags.HideAndDontSave;
			m_collider.convex = false;
		}
	}

	[ContextMenu("Clear")]
	public void ClearMesh()
	{
		Mesh.Hash = Guid.NewGuid().ToString();
		Mesh.Voxels.Clear();
	}

	[ContextMenu("Force Redraw")]
	public void ForceRedraw()
	{
		Invalidate(false);
	}

	public void Invalidate(bool forceCollider)
	{
		Debug.Log($"Invalidated {this}", gameObject);
		SetupComponents(forceCollider);
		if(!Mesh)
		{
			return;
		}
		if(MinLayer > MaxLayer)
		{
			MinLayer = MaxLayer;
		}
		m_filter.sharedMesh = Mesh.GenerateMeshInstance(null, MinLayer, MaxLayer);
		if(GenerateCollider)
		{
			m_collider.sharedMesh = m_filter.sharedMesh;
		}
		if(!CustomMaterials)
		{
			MeshRenderer.sharedMaterials = new[] { VoxelManager.Instance.DefaultMaterial, VoxelManager.Instance.DefaultMaterialTransparent, };
		}
		m_lastMeshHash = Mesh.Hash;
	}

	public Voxel? GetVoxel(int triangleIndex)
	{
		if(triangleIndex < 0)
		{
			return null;
		}

		int limit = triangleIndex * 3;
		int submesh;
		for (submesh = 0; submesh < m_filter.sharedMesh.subMeshCount; submesh++)
		{
			int numIndices = m_filter.sharedMesh.GetTriangles(submesh).Length;
			
			if (numIndices > limit)
				break;
			triangleIndex -= numIndices / 3;
			limit -= numIndices;
		}

		if(!Mesh.VoxelMapping.TryGetValue(submesh, out var innermap))
		{
			throw new Exception($"Couldn't find submesh mapping for {submesh}");
		}

		var triMapping = innermap[triangleIndex];
		var vox = Mesh.Voxels.Where(v => v.Key == triMapping.Coordinate);
		if(!vox.Any())
		{
			return null;
		}
		return vox.Single().Value;
	}
}
