using Common;
using Interaction.Activities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul;

namespace Actors
{
	public interface ILookAdapter
	{
		Transform transform { get; }
	}

	public class MovementController : ExtendedMonoBehaviour, IMovementController
	{
		public bool IsGrounded { get; private set; }
		public float TimeUngrounded { get; private set; }
		public Vector2 MoveDirection { get; private set; }

		private GravityManager GravityManager => GravityManager.Instance;
		public ILookAdapter LookAdapter { get; protected set; }
		public Rigidbody Rigidbody => GetComponent<Rigidbody>();
		public ActorState State => GetComponent<ActorState>();
		public Actor Actor => GetComponent<Actor>();

		[Header("Parameters")]
		public float MovementSpeed = 30;
		public float ThrusterSpeed = 60;
		public LayerMask CollisionMask = 1 << 8;
		public float RotateTowardGravitySpeed = 10;
		public float FreeFloatTime = .2f;
		public Transform GroundingPoint;
		public float PushOutSpeed = 10;

		protected bool m_inputJump;

		protected virtual void Start()
		{
			LookAdapter = gameObject.GetComponentByInterfaceInChildren<ILookAdapter>();
			Rigidbody.useGravity = false;
		}

		private void FixedUpdate()
		{
			var dt = Time.fixedDeltaTime;
			var gravityVec = GravityManager.GetGravityForce(transform.position);

			var groundingDistance = Vector3.Distance(GroundingPoint.position, transform.position);
			IsGrounded = Physics.Raycast(transform.position, gravityVec, out var groundHit, groundingDistance * 1.01f, CollisionMask, QueryTriggerInteraction.Ignore);
			var isGroundedForward = Physics.Raycast(transform.position + LookAdapter.transform.forward, gravityVec, out var forwardHit, groundingDistance, CollisionMask, QueryTriggerInteraction.Ignore);
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
				var straightenQuat = Quaternion.Euler(Quaternion.FromToRotation(Vector3.up, -gravityVec.normalized)
					.eulerAngles.xy().x0z(Rigidbody.rotation.eulerAngles.y));
				var straightenLerp = RotateTowardGravitySpeed * dt * (gravityVec.magnitude / 1000f);
				Rigidbody.rotation = Quaternion.Lerp(Rigidbody.rotation, straightenQuat, straightenLerp);
				//Rigidbody.rotation = straightenQuat;
				Debug.DrawLine(transform.position, transform.position + straightenQuat * transform.forward, Color.yellow);
			}

			{
				// Push out
				if (groundHit.distance < groundingDistance)
				{
					Rigidbody.MovePosition(Rigidbody.position + groundHit.normal * (groundingDistance - groundHit.distance) * dt * PushOutSpeed);
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

				var movement = MoveDirection;
				if (movement.magnitude > 0)
				{
					var localVelocityDirection = new Vector3(movement.x, 0, movement.y);
					var worldVelocityDirection = LookAdapter.transform.localToWorldMatrix.MultiplyVector(localVelocityDirection);

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
			if (Actor.Animator)
			{
				Actor.Animator.SetFloat("VelocityX", Rigidbody.velocity.x);
				Actor.Animator.SetFloat("VelocityY", Rigidbody.velocity.y);
				Actor.Animator.SetFloat("VelocityZ", Rigidbody.velocity.z);
			}
		}

		private void OnDrawGizmosSelected()
		{
			var gravityVec = GravityManager.GetGravityForce(transform.position);
			Gizmos.DrawLine(transform.position, GroundingPoint.position);
			Gizmos.DrawWireCube(GroundingPoint.position, Vector3.one * .05f);
		}

		public void Move(Vector2 dir)
		{
			Debug.Log($"Move: {dir}");
			MoveDirection = dir;
		}
	}
}