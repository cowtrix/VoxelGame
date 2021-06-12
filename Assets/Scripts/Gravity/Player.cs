using Common;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Voxul;

public class Player : MonoBehaviour
{
	public bool IsMoving =>
		(transform.localPosition - TargetPosition).sqrMagnitude > Threshold ||
		Quaternion.Angle(transform.localRotation, TargetRotation) > Threshold;
	public float MovementDistance = 1 / 3f;
	public float MovementSpeed = 1;
	public float Threshold = 0.1f;
	public Vector3 TargetPosition;
	public Quaternion TargetRotation;
	public VoxelRenderer Renderer;
	public LayerMask CollisionMask;

	private VoxelMaterial m_mat;

	private void Start()
	{
		TargetPosition = transform.localPosition;
		TargetRotation = transform.localRotation;
		m_mat = Renderer.Mesh.Voxels.First().Value.Material;
	}

	public void OnMove(InputAction.CallbackContext cntxt)
	{
		if (cntxt.started || cntxt.canceled || IsMoving)
		{
			return;
		}
		var move = cntxt.ReadValue<Vector2>().x0z();
		var dir = move.ClosestAxisNormal() * MovementDistance;
		Debug.DrawLine(transform.position, transform.position + dir * 10, Color.magenta, 5);
		bool colliding = false;
		var rootPos = transform.position;
		foreach (var vox in Renderer.Mesh.Voxels.ToList())
 		{
			var pos = Renderer.transform.localToWorldMatrix.MultiplyPoint3x4(vox.Key.ToVector3());
			var scale = VoxelCoordinate.LayerToScale(vox.Key.Layer);
			var col = colliding ? Color.red : Color.white;
			if (Physics.Raycast(pos, dir, out var hit, scale, CollisionMask))
			{
				Debug.Log($"Hit {hit.collider}", hit.collider);
				var pickup = hit.collider.GetComponent<Pickup>();
				if (pickup)
				{
					Debug.Log($"Maybe pickup {pickup}", pickup);
					DoPickup(pickup, hit);
				}
				colliding = true;
			}
			Debug.DrawLine(rootPos, pos, col, 1);
			DebugHelper.DrawSphere(hit.point, Quaternion.identity, .02f, Color.cyan, 2);
			DebugHelper.DrawCube(pos, Vector3.one * scale * .5f, transform.rotation, col, 1);
		}
		if (!colliding)
		{
			TargetPosition += dir;
		}
	}

	void DoPickup(Pickup pickup, RaycastHit hit)
	{
		var pickupRenderer = pickup.GetComponent<VoxelRenderer>();
		if (!VoxelCoordinate.VectorToDirection(hit.normal, out var voxelHitDir))
		{
			return;
		}
		var hitVox = pickupRenderer.GetVoxel(hit.triangleIndex);
		if (!hitVox.HasValue)
		{
			return;
		}
		var surface = hitVox.Value.Material.GetSurface(voxelHitDir);
		if(surface != m_mat.Default)
		{
			return;
		}

		foreach (var pickupVox in pickupRenderer.Mesh.Voxels)
		{
			var relVec = transform.worldToLocalMatrix.MultiplyPoint3x4(pickup.transform.localToWorldMatrix.MultiplyPoint3x4(pickupVox.Key.ToVector3()));
			var newVox = VoxelCoordinate.FromVector3(relVec, pickupVox.Key.Layer);
			Debug.Log("New vox: " + newVox);
			Renderer.Mesh.Voxels.AddSafe(new Voxel
			{
				Coordinate = newVox,
				Material = m_mat
			});
		}
		Destroy(pickup.gameObject);
		Renderer.Mesh.Invalidate();
		Renderer.Invalidate(false);
	}

	public void Update()
	{
		VoxelManager.Instance.DefaultMaterial.SetVector("PlayerPosition", transform.position);
		VoxelManager.Instance.DefaultMaterialTransparent.SetVector("PlayerPosition", transform.position);
		var dt = Time.deltaTime;
		transform.localRotation = Quaternion.RotateTowards(transform.localRotation, TargetRotation, MovementSpeed * dt);
		transform.localPosition = Vector3.MoveTowards(transform.localPosition, TargetPosition, MovementSpeed * dt);
	}
}
