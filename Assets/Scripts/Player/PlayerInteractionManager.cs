using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionManager : MonoBehaviour
{
	public Transform GravCursor;
	public float MaximumInteractionDistance = 1000;
	public LayerMask InteractionLayerMask;

	private Dictionary<Interactable, Func<Vector3>> m_secondaryInteractables
		= new Dictionary<Interactable, Func<Vector3>>();

	public void AddSecondaryInteractable(Interactable interactable, Func<Vector3> position)
	{
		m_secondaryInteractables[interactable] = position;
	}

	public void RemoveSecondaryInteractable(Interactable interactable)
	{
		m_secondaryInteractables.Remove(interactable);
	}

	public PlayerInput PlayerInput;
	public CameraController CameraController => CameraController.Instance;
	public Interactable FocusedInteractable;

	bool m_gravDown;
	RaycastHit? m_currentHit;

	public void OnGravity(InputAction.CallbackContext cntxt)
	{
		m_gravDown = cntxt.ReadValue<float>() > 0;
		Debug.Log($"Set Grav Down to {m_gravDown}");
	}

	public void OnFire(InputAction.CallbackContext cntxt)
	{
		if (m_currentHit == null || !cntxt.started)
		{
			return;
		}
		Debug.Log("Fire!");
		var hit = m_currentHit.Value;
		if (m_gravDown)
		{
			Debug.Log($"Set gravity to {hit.normal}");
			GravityManager.Instance.DefaultGravity = -hit.normal;
		}
		else
		{
			var destroyable = hit.collider.GetComponent<DestroyableVoxel>();
			if (destroyable)
			{
				Debug.Log($"Found destroyable : {hit.collider}", hit.collider);
				//destroyable.Hit(hit.collider.transform.position, hit.point, 1, .1f);
				destroyable.Hit(hit.triangleIndex, 1);
			}
			else
			{
				Debug.Log($"Nothing happened: {hit.collider}", hit.collider);
			}
		}
	}

	private void Update()
	{
		var ray = new Ray(transform.position, CameraController.transform.forward);
		if (!Physics.Raycast(ray, out var hit, MaximumInteractionDistance, InteractionLayerMask, QueryTriggerInteraction.Ignore))
		{
			Debug.DrawRay(ray.origin, ray.direction * MaximumInteractionDistance, Color.red);
			m_currentHit = null;
			return;
		}
		Debug.DrawLine(transform.position, hit.point, Color.green);
		m_currentHit = hit;
		GravCursor.position = hit.point;
		GravCursor.rotation = Quaternion.LookRotation(hit.normal);
		GravCursor.gameObject.SetActive(m_gravDown);
	}
}
