using Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul;

public class Pickup : TrackedObject<Pickup>
{
    private VoxelRenderer Renderer => GetComponent<VoxelRenderer>();
	public List<(Vector3, Vector3)> m_attachmentPoints = new List<(Vector3, Vector3)>();
	public List<(Vector3, Vector3)> m_activeAttachmentPoints = new List<(Vector3, Vector3)>();

	public LayerMask LayerMask;
	public ParticleSystem Connector;
	public int EmissionSpeed = 1;
	public int Level;

	public override bool TrackDisabled => true;

	private void Start()
	{
		foreach (var vox in Renderer.Mesh.Voxels)
		{
			foreach(var dirSurf in vox.Value.Material.GetSurfaces()
				.Where(s => s.Item2 == Player.Instance.PlayerPickup.Data.Default))
			{
				m_attachmentPoints.Add((vox.Key.ToVector3(), 
					VoxelCoordinate.DirectionToCoordinate(dirSurf.Item1, 0).ToVector3()));
			}
		}
		StartCoroutine(CheckAttachement());
	}

	private IEnumerator CheckAttachement()
	{
		while (true)
		{
			m_activeAttachmentPoints.Clear();
			foreach (var attachmentPoint in m_attachmentPoints)
			{
				var origin = transform.localToWorldMatrix.MultiplyPoint3x4(attachmentPoint.Item1);
				var dir = transform.localToWorldMatrix.MultiplyVector(attachmentPoint.Item2);
				if (Physics.Raycast(origin,
					dir.normalized,
					out var hit,
					dir.magnitude,
					LayerMask))
				{
					Debug.DrawLine(origin, hit.point, Color.magenta, 0);
					m_activeAttachmentPoints.Add(attachmentPoint);
				}
				Debug.DrawLine(origin, origin + dir, Color.cyan, 0);
			}
			yield return new WaitForSeconds(1);
		}
		
	}

	private void Update()
	{
		foreach (var attachmentPoint in m_activeAttachmentPoints)
		{
			var origin = transform.localToWorldMatrix.MultiplyPoint3x4(attachmentPoint.Item1);
			var dir = transform.localToWorldMatrix.MultiplyVector(attachmentPoint.Item2);
			Connector.transform.position = origin;
			Connector.transform.rotation = Quaternion.LookRotation(dir.normalized);
			Connector.Emit(EmissionSpeed);
		}
		
	}
}
