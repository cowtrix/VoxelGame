using MadMaps.Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
	public Transform CameraTransform;
    public PlayerInput Input;
	public Rigidbody Rigidbody;

	[Header("Parameters")]
	public float MovementSpeed = 1f;
	public float StrafeSpeed = .7f;
	public GravitySource GravitySource;
	public Vector3 GravityVector = new Vector3(0, -1, 0);
	public Vector3 Jetpack = new Vector3(0, 2, 0);
	public Vector3 Jump = new Vector3(0, 2, 0);
	public float GroundCastDistance = 10f;
	public float GravityDamper = 1.1f;
	public float HoverHeight = .5f;
	public LayerMask LayerMask;
	public float RotateSpeed = 1f;

	private InputAction m_move, m_jump;

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
	}

	private void FixedUpdate()
	{
		var dt = Time.fixedDeltaTime;
		var gravityVec = GetGravityVector();
		var castVector = transform.localToWorldMatrix.MultiplyVector(gravityVec.normalized * GroundCastDistance * transform.lossyScale.x);
		Debug.DrawLine(transform.position, transform.position + castVector, Color.white);
		if (Physics.Raycast(transform.position, -transform.up, out var hitInfo, GroundCastDistance * transform.lossyScale.x, LayerMask))
		{
			var hoverVector = transform.localToWorldMatrix.MultiplyVector(Vector3.down * HoverHeight);
			Debug.DrawLine(transform.position + new Vector3(.1f, 0, .1f), transform.position + hoverVector + new Vector3(.1f, 0, .1f), Color.green);
			Debug.DrawLine(transform.position, hitInfo.point, Color.magenta);
			if(hitInfo.distance < hoverVector.magnitude)
			{
				Rigidbody.AddForce(-gravityVec * dt * GravityDamper);
			}
		}

		Rigidbody.MoveRotation(Quaternion.Lerp(Rigidbody.rotation, 
			Quaternion.LookRotation(Vector3.forward, -gravityVec), RotateSpeed * dt));

		Rigidbody.AddForce(gravityVec * dt);
		/*if(m_jump.triggered)
		{
			Debug.Log($"Jumping");
			Rigidbody.AddForce(transform.localToWorldMatrix.MultiplyVector(Jump));
		}*/
		if(m_jump.ReadValue<float>() > 0)
		{
			Rigidbody.AddForce(transform.localToWorldMatrix.MultiplyVector(Jetpack) * dt);
		}

		var movement = m_move.ReadValue<Vector2>();
		Rigidbody.velocity += CameraTransform.transform.localToWorldMatrix.MultiplyVector(
			new Vector3(movement.x * StrafeSpeed, 0, movement.y * MovementSpeed) * dt);
	}
}
