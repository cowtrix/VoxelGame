using Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
	public SmoothPositionVector3 SmoothPosition { get; private set; }

	public CameraController CameraController => CameraController.Instance;
    public PlayerInput Input;
	public Rigidbody Rigidbody;

	[Header("Parameters")]
	public float MovementSpeed = 1f;
	public float StrafeSpeed = .7f;
	public GravitySource GravitySource;
	public Vector3 GravityVector = new Vector3(0, -1, 0);
	public AnimationCurve Jetpack;
	public Vector3 Jump = new Vector3(0, 2, 0);
	public float GroundCastDistance = 10f;
	public AnimationCurve GravityDamper;
	public float HoverHeight = .5f;
	public LayerMask LayerMask;
	public float RotateSpeed = 1f;

	private InputAction m_move, m_jump;
	private float m_jumpTime;
	public bool IsGrounded;

	private Vector3 GetGravityVector()
	{
		if(GravitySource)
		{
			return (transform.position - GravitySource.transform.position).normalized * GravitySource.Strength;
		}
		return GravityVector;
	}

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
		var gravityVec = GetGravityVector();
		var scaleFactor = transform.lossyScale.x;
		var castVector = transform.localToWorldMatrix.MultiplyVector(gravityVec.normalized * GroundCastDistance * scaleFactor);
		Debug.DrawLine(transform.position, transform.position + castVector, Color.white);
		if (Physics.Raycast(transform.position, -transform.up, out var hitInfo, GroundCastDistance * scaleFactor, LayerMask))
		{
			var hoverVector = transform.localToWorldMatrix.MultiplyVector(Vector3.down * HoverHeight);
			Debug.DrawLine(transform.position + new Vector3(.1f, 0, .1f), transform.position + hoverVector + new Vector3(.1f, 0, .1f), Color.green);
			Debug.DrawLine(transform.position, hitInfo.point, Color.magenta);
			
			if(hitInfo.distance < HoverHeight * scaleFactor)
			{
				if (!Physics.Raycast(transform.position, transform.up, out var uphitInfo, HoverHeight * scaleFactor * .75f, LayerMask))
				{
					var percent = 1 - (hitInfo.distance / hoverVector.magnitude);
					Rigidbody.AddForce(-gravityVec * dt * GravityDamper.Evaluate(percent) * scaleFactor);
					IsGrounded = true;
				}
				else
				{
					Debug.DrawLine(uphitInfo.point, transform.position, Color.magenta);
				}
			}
		}

		Rigidbody.mass = scaleFactor;
		Rigidbody.ResetCenterOfMass();

		Rigidbody.MoveRotation(Quaternion.Lerp(Rigidbody.rotation, 
			Quaternion.LookRotation(Vector3.forward, -gravityVec), RotateSpeed * dt));

		Rigidbody.AddForce(gravityVec * dt * scaleFactor);
		/*if(m_jump.triggered)
		{
			Debug.Log($"Jumping");
			Rigidbody.AddForce(transform.localToWorldMatrix.MultiplyVector(Jump));
		}*/
		
		if(m_jump.ReadValue<float>() > 0)
		{
			if (!IsGrounded)
			{
				m_jumpTime = 1;
			}
			var amount = Jetpack.Evaluate(m_jumpTime);
			Rigidbody.AddForce(transform.localToWorldMatrix.MultiplyVector(amount * Vector3.up) * dt);
			m_jumpTime += dt;
		}
		else
		{
			m_jumpTime = 0;
		}

		var movement = m_move.ReadValue<Vector2>();
		if(CameraController.IsFreeLook)
		{
			Rigidbody.velocity += transform.localToWorldMatrix.MultiplyVector(
				new Vector3(movement.x * StrafeSpeed, 0, movement.y * MovementSpeed) * dt);
		}
		else
		{
			Rigidbody.velocity += CameraController.transform.localToWorldMatrix.MultiplyVector(
				new Vector3(movement.x * StrafeSpeed, 0, movement.y * MovementSpeed) * dt);
		}
		SmoothPosition.Push(Rigidbody.position);
	}
}
