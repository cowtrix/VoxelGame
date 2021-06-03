
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : Singleton<MovementController>
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

	private Vector2 m_inputLook;
	private bool m_inputJump;
	private int m_jumpCount;

	public SmoothPositionVector3 SmoothPosition { get; private set; }

	private void Start()
	{
		Rigidbody.useGravity = false;
		SmoothPosition = new SmoothPositionVector3(10, transform.position);
	}

	public void OnMove(InputAction.CallbackContext context)
	{
		m_inputLook = context.ReadValue<Vector2>();
	}

	public void OnJump(InputAction.CallbackContext context)
	{
		var val = context.ReadValue<float>();
		Debug.Log($"OnJump: {val}");
		m_inputJump = val > .5f && context.canceled;
	}

	private void FixedUpdate()
	{
		var dt = Time.fixedDeltaTime;
		var gravityVec = GravityManager.GetGravityForce(transform.position);

		IsGrounded = Physics.Raycast(transform.position, gravityVec, out var groundHit, GroundingDistance, CollisionMask, QueryTriggerInteraction.Ignore);
		Debug.DrawLine(transform.position, transform.position + gravityVec * dt, Color.green);
		if(IsGrounded)
		{
			m_jumpCount = 2;
		}

		{
			// Straighten up
			var straightenQuat = Quaternion.FromToRotation(Vector3.up, -gravityVec.normalized);
			var straightenLerp = RotateTowardGravitySpeed * dt * (gravityVec.magnitude / 1000f);
			Rigidbody.rotation = Quaternion.Lerp(Rigidbody.rotation, straightenQuat, straightenLerp);
			//Rigidbody.rotation = straightenQuat;
			Debug.DrawLine(transform.position, transform.position + straightenQuat * transform.forward, Color.yellow);
		}

		{
			// Move from input
			if (m_jumpCount > 0 && m_inputJump)
			{
				Debug.Log($"Jumping");
				Rigidbody.AddForce(transform.localToWorldMatrix.MultiplyVector(JumpStrength) * Rigidbody.mass);
				m_jumpCount--;
				m_inputJump = false;
			}

			var movement = m_inputLook;
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
