using Common;
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

	private void Start()
	{
		TargetPosition = transform.localPosition;
		TargetRotation = transform.localRotation;
	}

	public void OnMove(InputAction.CallbackContext cntxt)
	{
		if (cntxt.started || cntxt.canceled || IsMoving)
		{
			return;
		}
		var move = cntxt.ReadValue<Vector2>().x0z();
		var dir = move.ClosestAxisNormal() * MovementDistance;
		bool colliding = false;
		var rootPos = transform.position;
		foreach (var vox in Renderer.Mesh.Voxels)
		{
			var pos = transform.localToWorldMatrix * (vox.Key.ToVector3() + dir);
			var scale = VoxelCoordinate.LayerToScale(vox.Key.Layer);
			var col = colliding ? Color.red : Color.white;
			if (Physics.Raycast(rootPos, pos, scale, CollisionMask))
			{
				colliding = true;
			}
			var emitParams = new ParticleSystem.EmitParams
			{
				position = pos,
				startColor = col,
			};
			Debug.DrawLine(rootPos, pos, col, 1);
			DebugHelper.DrawCube(pos, Vector3.one * scale * .5f, transform.rotation, col, 1);
		}
		if (!colliding)
		{
			TargetPosition += dir;
		}
	}

	public void Update()
	{
		var dt = Time.deltaTime;
		transform.localRotation = Quaternion.RotateTowards(transform.localRotation, TargetRotation, MovementSpeed * dt);
		transform.localPosition = Vector3.MoveTowards(transform.localPosition, TargetPosition, MovementSpeed * dt);
	}
}
