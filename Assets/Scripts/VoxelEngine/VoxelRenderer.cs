using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteAlways]
public class VoxelRenderer : MonoBehaviour
{
	public VoxelMesh Mesh;

	private MeshFilter m_filter;
	private MeshRenderer m_renderer;
	private MeshCollider m_collider;

	[SerializeField]
	[HideInInspector]
	private string m_lastMeshHash;

	private void Start()
	{
		Invalidate();
	}

	private void Update()
	{
		if(Mesh.Hash == m_lastMeshHash)
		{
			return;
		}
		Invalidate();
	}

	private void SetupComponents()
	{
		if (!m_filter)
		{
			m_filter = gameObject.GetOrAddComponent<MeshFilter>();
		}
		m_filter.hideFlags = HideFlags.HideAndDontSave;
		if (!m_renderer)
		{
			m_renderer = gameObject.GetOrAddComponent<MeshRenderer>();
		}
		m_renderer.hideFlags = HideFlags.HideAndDontSave;
		if (!m_collider)
		{
			m_collider = gameObject.GetOrAddComponent<MeshCollider>();
		}
		m_collider.hideFlags = HideFlags.HideAndDontSave;
		m_collider.convex = false;
	}

	[ContextMenu("Clear")]
	public void ClearMesh()
	{
		Mesh.Hash = Guid.NewGuid().ToString();
		Mesh.Voxels.Clear();
	}

	[ContextMenu("Force Redraw")]
	public void Invalidate()
	{
		SetupComponents();

		m_filter.sharedMesh = Mesh.GenerateMeshInstance();
		m_collider.sharedMesh = m_filter.sharedMesh;		
		m_renderer.sharedMaterial = VoxelManager.Instance.DefaultMaterial;

		m_lastMeshHash = Mesh.Hash;
	}

	public Voxel? GetVoxel(int triangleIndex)
	{
		if(triangleIndex < 0)
		{
			return null;
		}	
		var mapping = Mesh.VoxelMapping[triangleIndex];
		var vox = Mesh.Voxels.Where(v => v.Key == mapping.Coordinate);
		if(!vox.Any())
		{
			return null;
		}
		return vox.Single().Value;
	}
}
