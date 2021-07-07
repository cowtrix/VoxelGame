using Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Voxul;

public class Player : Singleton<Player>
{
	public AudioSource AudioSource => GetComponent<AudioSource>();
	public AudioSource WindLoop, Tinkle;
	public AudioClip CantMove, Land, MoveScrape;

	public byte SnapLayer = 1;
	public bool IsMoving =>
		(transform.localPosition - TargetPosition).magnitude > MovementThreshold ||
		Quaternion.Angle(transform.rotation, TargetRotation) > RotationThreshold ||
		(GMTKGameManager.Instance.CurrentCheckpoint && GMTKGameManager.Instance.CurrentCheckpoint.IsMoving);
	public float MovementDistance => SnapLayer / (float)VoxelCoordinate.LayerRatio;
	public float MovementSpeed = 1;
	public float MovementThreshold = 0.01f;
	public float RotationSpeed = 1;
	public float RotationThreshold = 0.1f;
	public Vector3 TargetPosition;
	public Quaternion TargetRotation;
	public float RotationCheckDistance = .8f;
	public VoxelRenderer Renderer;
	public LayerMask CollisionMask;
	public VoxelMaterialAsset PlayerPickup;
	private VoxelMaterial m_mat => PlayerPickup.Data;
	private bool m_isFalling;

	private void Start()
	{
		TargetPosition = transform.localPosition;
		TargetRotation = transform.rotation;
	}

	public void OnRotate(InputAction.CallbackContext cntxt)
	{
		if (cntxt.started || cntxt.canceled || IsMoving)
		{
			return;
		}

		if (!Help.Instance.Dismissed)
		{
			Help.Instance.Dismissed = true;
			Help.Instance.DismissedOnce = true;
		}

		if (cntxt.action.name == "RotateClockwise")
		{
			TryRotate(Quaternion.Euler(new Vector3(0, -90, 0)));
		}
		else
		{
			TryRotate(Quaternion.Euler(new Vector3(0, 90, 0)));
		}
	}

	private bool TryRotate(Quaternion rot)
	{
		const float degreeCheck = 1 / 10f;
		var lastLocalRot = Quaternion.identity;
		for(var i = 0f; i < 1; i += degreeCheck)
		{
			var localLerpRot = Quaternion.Lerp(Quaternion.identity, rot, i);
			//Debug.Log($"lerpRot = {localLerpRot.eulerAngles}\ti = {i}\t");
			foreach (var vox in Renderer.Mesh.Voxels.ToList())
			{
				var pos =  Renderer.transform.localToWorldMatrix.MultiplyPoint3x4(localLerpRot * vox.Key.ToVector3());
				var lastPos = Renderer.transform.localToWorldMatrix.MultiplyPoint3x4(lastLocalRot * vox.Key.ToVector3());

				var dir = (pos - lastPos).normalized;
				var scale = VoxelCoordinate.LayerToScale(vox.Key.Layer);
				var col = Color.white;
				if (Physics.Raycast(lastPos, dir, out var hit, scale * RotationCheckDistance, CollisionMask))
				{
					col = Color.red;
					Debug.Log($"Hit {hit.collider}", hit.collider);
					DebugHelper.DrawSphere(hit.point, Quaternion.identity, .02f, Color.cyan, 20);
					AudioSource.PlayOneShot(CantMove, .5f);
					return false;
				}
				else
				{
					DebugHelper.DrawSphere(lastPos, Quaternion.identity, .02f, Color.magenta, 20);
				}
				Debug.DrawLine(lastPos, lastPos + dir * scale * RotationCheckDistance, Color.Lerp(Color.red, Color.blue, i), 20);
				DebugHelper.DrawCube(pos, Vector3.one * scale * .5f, localLerpRot * transform.rotation, col.WithAlpha(.1f), 20);
			}
			lastLocalRot = localLerpRot;
		}
		TargetRotation *= rot;
		GMTKGameManager.Instance.CheckWin();
		return true;
	}

	public void OnMove(InputAction.CallbackContext cntxt)
	{
		if (cntxt.started || cntxt.canceled || IsMoving)
		{
			return;
		}

		if (!Help.Instance.Dismissed)
		{
			Help.Instance.Dismissed = true;
			Help.Instance.DismissedOnce = true;
		}

		var move = transform.localToWorldMatrix.MultiplyVector(cntxt.ReadValue<Vector2>().x0z());
		var dir = move.ClosestAxisNormal() * MovementDistance;
		Debug.Log($"Moved {dir}");
		Debug.DrawLine(transform.position, transform.position + dir * 10, Color.magenta, 5);
		bool colliding = false;
		var rootPos = transform.position;
		foreach (var vox in Renderer.Mesh.Voxels.ToList())
 		{
			var pos = Renderer.transform.localToWorldMatrix.MultiplyPoint3x4(vox.Key.ToVector3());
			var scale = VoxelCoordinate.LayerToScale(vox.Key.Layer);
			if (Physics.Raycast(pos, dir, out var hit, scale, CollisionMask))
			{
				var pickup = hit.collider.GetComponent<Pickup>();
				if (pickup)
				{
					Debug.Log($"Maybe pickup {pickup}", pickup);
					DoPickup(pickup, hit, scale);
				}
				if (hit.distance < scale)
				{
					Debug.Log($"Can't move because of {hit.collider}", hit.collider);
					DebugHelper.DrawSphere(hit.point, Quaternion.identity, .02f, Color.cyan, 2);
					colliding = true;
				}
			}
			var col = colliding ? Color.red : Color.white;
			Debug.DrawLine(rootPos, pos, col, 1);
			DebugHelper.DrawCube(pos, Vector3.one * scale * .5f, transform.rotation, col, 1);
		}
		if (!colliding)
		{
			TargetPosition += dir;
			AudioSource.PlayOneShot(MoveScrape, .05f);
		}
		else
		{
			AudioSource.PlayOneShot(CantMove, .2f);
		}
	}

	void DoPickup(Pickup pickup, RaycastHit hit, float dist)
	{
		var pickupRenderer = pickup.GetComponent<VoxelRenderer>();
		var localDir = pickup.transform.worldToLocalMatrix.MultiplyVector(hit.normal);
		if (!VoxelCoordinate.VectorToDirection(localDir, out var voxelHitDir))
		{
			return;
		}
		var hitVox = pickupRenderer.GetVoxel(hit.collider, hit.triangleIndex);
		if (!hitVox.HasValue)
		{
			return;
		}
		var surface = hitVox.Value.Material.GetSurface(voxelHitDir);

		if(hit.distance < dist)
		{
			if (surface != m_mat.Default)
			{
				Debug.Log($"Pickup had wrong surface. Direction was {voxelHitDir}", pickup);
				return;
			}

			foreach (var pickupVox in pickupRenderer.Mesh.Voxels)
			{
				var relVec = Renderer.transform.worldToLocalMatrix.MultiplyPoint3x4(
					pickup.transform.localToWorldMatrix.MultiplyPoint3x4(pickupVox.Key.ToVector3()));
				var newVox = VoxelCoordinate.FromVector3(relVec, pickupVox.Key.Layer);
				Debug.Log("New vox: " + newVox);
				Renderer.Mesh.Voxels.AddSafe(new Voxel
				{
					Coordinate = newVox,
					Material = m_mat
				});
			}
			pickup.gameObject.SetActive(false);
			Renderer.Mesh.Invalidate();
			Renderer.Invalidate(true, false);
		}
	}

	public void Update()
	{
		if (Help.Instance.ToggleContainer.activeInHierarchy)
		{
			return;
		}
		WindLoop.volume = Mathf.MoveTowards(WindLoop.volume, GMTKGameManager.Instance.CurrentCheckpoint && IsMoving ? .1f : 0f, Time.deltaTime * .5f);
		var activePickups = Pickup.Instances.Where(f => f.gameObject.activeInHierarchy && f.Enabled).ToList();
		foreach(var p in activePickups)
		{
			Debug.Log($"Pickup active: {p}", p);
		}
		Tinkle.volume = Mathf.MoveTowards(Tinkle.volume, activePickups.Any() ? .3f : 0f, Time.deltaTime * .5f);
		GMTKGameManager.Instance.CheckWin();

		TargetPosition = TargetPosition.RoundToIncrement(SnapLayer / (float)VoxelCoordinate.LayerRatio);

		VoxelManager.Instance.DefaultMaterial.SetVector("PlayerPosition", transform.position);
		VoxelManager.Instance.DefaultMaterialTransparent.SetVector("PlayerPosition", transform.position);
		var dt = Time.deltaTime;
		transform.rotation = Quaternion.RotateTowards(transform.rotation, TargetRotation, RotationSpeed * dt);
		transform.localPosition = Vector3.MoveTowards(transform.localPosition, TargetPosition, MovementSpeed * dt);

		if (IsMoving)
		{
			return;
		}

		bool shouldFall = true;
		foreach (var vox in Renderer.Mesh.Voxels.Keys)
		{
			var pos = Renderer.transform.localToWorldMatrix.MultiplyPoint3x4(vox.ToVector3());
			var scale = VoxelCoordinate.LayerToScale(vox.Layer);
			if (Physics.Raycast(pos, Vector3.down, out var hit, 10000, CollisionMask) && hit.distance <= scale * 1.2f)
			{
				shouldFall = false;
			}
		}
		if (shouldFall)
		{
			Debug.Log("Fell down a block");
			m_isFalling = true;
			TargetPosition -= Vector3.up * MovementDistance;
		}
		else if(m_isFalling)
		{
			m_isFalling = false;
			AudioSource.PlayOneShot(Land, .4f);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawLine(Vector3.zero, TargetPosition);
	}
}
