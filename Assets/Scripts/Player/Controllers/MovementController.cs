
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Voxul;

public class MovementController : ExtendedMonoBehaviour
{
	public bool IsGrounded { get; private set; }
	public float TimeUngrounded { get; private set; }

	private GravityManager GravityManager => GravityManager.Instance;
	private CameraController CameraController => CameraController.Instance;
	private PlayerInput Input => GetComponent<PlayerInput>();
	public Rigidbody Rigidbody => GetComponent<Rigidbody>();
	public ActorState State => GetComponent<ActorState>();

	[Header("Parameters")]
	public float MovementSpeed = 100f;
	public float ThrusterSpeed = 100f;
	public LayerMask CollisionMask;
	public float RotateTowardGravitySpeed = 1f;
	public float FreeFloatTime = .2f;
	public float GroundingDistance = 1;
	public float PushOutSpeed = 1;

	private Vector2 m_inputLook;
	private bool m_inputJump;

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
		if (val > .5f)
		{
			m_inputJump = true;
		}
		if (context.canceled)
		{
			m_inputJump = false;
		}
		Debug.Log($"Jump: {m_inputJump}");
	}

	private void FixedUpdate()
	{
		var dt = Time.fixedDeltaTime;
		var gravityVec = GravityManager.GetGravityForce(transform.position);

		IsGrounded = Physics.Raycast(transform.position, gravityVec, out var groundHit, GroundingDistance, CollisionMask, QueryTriggerInteraction.Ignore);
		var isGroundedForward = Physics.Raycast(transform.position + CameraController.transform.forward, gravityVec, out var forwardHit, GroundingDistance, CollisionMask, QueryTriggerInteraction.Ignore);
		Debug.DrawLine(transform.position, transform.position + gravityVec * dt, Color.green);

		if (!IsGrounded)
		{
			Rigidbody.AddForce(gravityVec * dt * Rigidbody.mass);
			TimeUngrounded += dt;
		}
		else
		{
			TimeUngrounded = 0;
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
			// Push out
			if (groundHit.distance < GroundingDistance)
			{
				Rigidbody.MovePosition(Rigidbody.position + groundHit.normal * (GroundingDistance - groundHit.distance) * dt * PushOutSpeed);
			}
		}

		{
			// Move from input
			if (m_inputJump)
			{
				// Do jump boost
				if (!State.TryGetValue<float>(nameof(PlayerState.ThrusterFuel), out var thrusterFuel) || thrusterFuel > .1f)
				{
					Rigidbody.AddForce(transform.localToWorldMatrix.MultiplyVector(ThrusterSpeed * transform.up) * Rigidbody.mass);
					State.TryAdd(nameof(PlayerState.ThrusterFuel), nameof(PlayerState.ThrusterEfficiency));
				}
			}

			var movement = m_inputLook;
			if(movement.magnitude > 0)
			{
				var localVelocityDirection = new Vector3(movement.x, 0, movement.y);
				var worldVelocityDirection = CameraController.transform.localToWorldMatrix.MultiplyVector(localVelocityDirection);

				if (IsGrounded)
				{
					var normal = isGroundedForward ? forwardHit.normal : groundHit.normal;
					worldVelocityDirection -= normal * Vector3.Dot(worldVelocityDirection, normal);
				}
				else
				{

					if (TimeUngrounded > FreeFloatTime && (!State.TryGetValue<float>(nameof(PlayerState.ThrusterFuel), out var thrusterFuel) || thrusterFuel > .1f))
					{
						State.TryAdd(nameof(PlayerState.ThrusterFuel), nameof(PlayerState.ThrusterEfficiency));
					}
					else
					{
						worldVelocityDirection *= 0.01f;
					}
				}

				Debug.DrawLine(transform.position, transform.position + worldVelocityDirection, Color.cyan, 5);
				Rigidbody.velocity += worldVelocityDirection.normalized * (IsGrounded ? MovementSpeed : ThrusterSpeed) * dt;
			}
		}
		SmoothPosition.Push(Rigidbody.position);
	}
}
