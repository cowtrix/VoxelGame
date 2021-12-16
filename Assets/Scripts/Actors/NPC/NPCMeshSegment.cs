using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul;
using Voxul.Meshing;
using Common;

[RequireComponent(typeof(VoxelRenderer))]
public class NPCMeshSegment : MonoBehaviour
{
	public TriColorSet.eColorMode ColorMode;
	[Range(0, 2)]
	public float Saturation = 1f;
	public Vector2 Scale = new Vector2(1, 1);
	public ObjectSet Collection;
	public VoxelRenderer Renderer => GetComponent<VoxelRenderer>();
	public NPCMeshManager NPCManager => transform.GetComponentInAncestors<NPCMeshManager>();
	protected IEnumerable<NPCMeshSegment> Children => GetComponentsInChildren<NPCMeshSegment>()
		.Where(t => t.transform.parent == transform);
}
