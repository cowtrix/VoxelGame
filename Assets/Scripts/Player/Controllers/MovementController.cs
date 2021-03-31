
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
	private GravityManager GravityManager => GravityManager.Instance;
	private CameraController CameraController => CameraController.Instance;

	[Header("References")]
	public PlayerInput Input;
	public Rigidbody Rigidbody;

	[Header("Parameters")]
	public float MovementSpeed = 100f;
	public float StrafeSpeed = 100f;
	public Vector3 JumpStrength = new Vector3(0, 500, 0);
	public LayerMask CollisionMask;
	public float RotateTowardGravitySpeed = 1f;

	public float GroundingDistance = 1;
	public bool IsGrounded;

	public SmoothPositionVector3 SmoothPosition { get; private set; }

	private InputAction m_move, m_jump;

	private void Start()
	{
		m_move = Input.actions.Single(a => a.name == "Move");
		m_jump = Input.actions.Single(a => a.name == "Jump");
		Rigidbody.useGravity = false;
		Cursor.lockState = CursorLockMode.Locked;
		SmoothPosition = new SmoothPositionVector3(10, transform.position);
	}

	private void FixedUpdate()
	{
		var dt = Time.fixedDeltaTime;
		var gravityVec = GravityManager.GetGravityForce(transform.position);

		IsGrounded = Physics.Raycast(transform.position, gravityVec, out var groundHit, GroundingDistance, CollisionMask, QueryTriggerInteraction.Ignore);
		Debug.DrawLine(transform.position, transform.position + gravityVec, Color.green);

		{
			// Straighten up
			var straightenQuat = Quaternion.FromToRotation(Vector3.up, -gravityVec.normalized);
			var straightenLerp = RotateTowardGravitySpeed * dt;
			Rigidbody.rotation = Quaternion.Lerp(Rigidbody.rotation, straightenQuat, straightenLerp);
			//Rigidbody.rotation = straightenQuat;
			Debug.DrawLine(transform.position, transform.position + straightenQuat * transform.forward, Color.yellow);
		}

		{
			// Move from input
			if (m_jump.triggered)
			{
				Debug.Log($"Jumping");
				Rigidbody.AddForce(transform.localToWorldMatrix.MultiplyVector(JumpStrength) * Rigidbody.mass);
			}

			var movement = m_move.ReadValue<Vector2>();
			var localVelocityDirection = new Vector3(movement.x, 0, movement.y);
			var worldVelocityDirection = CameraController.transform.localToWorldMatrix.MultiplyVector(localVelocityDirection);
			if (IsGrounded)
			{
				Debug.DrawLine(transform.position, groundHit.point, Color.white, 5);
				Debug.DrawLine(transform.position, transform.position + worldVelocityDirection, Color.magenta, 5);
				worldVelocityDirection -= groundHit.normal * Vector3.Dot(worldVelocityDirection, groundHit.normal);
			}
			else
			{
				Rigidbody.AddForce(gravityVec * dt * Rigidbody.mass);
			}
			Debug.DrawLine(transform.position, transform.position + worldVelocityDirection, Color.cyan, 5);
			Rigidbody.velocity += worldVelocityDirection.normalized * MovementSpeed * dt;
		}
		SmoothPosition.Push(Rigidbody.position);
	}
}
