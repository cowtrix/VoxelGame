using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionManager : MonoBehaviour
{
	public float MaximumInteractionDistance = 1000;
	public float FocusCubeSize = .1f;
	public Transform FocusCube;
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

	private InputAction m_useAction;

	private void Start()
	{
		m_useAction = PlayerInput.actions.Single(a => a.name == "Use");
	}

	private void Update()
	{
		Interactable newInteractable = null;
		if (Physics.Raycast(transform.position, (CameraController.FocusPoint - transform.position).normalized, out var hit,
			MaximumInteractionDistance * transform.lossyScale.x,  InteractionLayerMask, QueryTriggerInteraction.Collide))
		{
			newInteractable = hit.collider.GetComponent<Interactable>();
		}
		if(FocusedInteractable != newInteractable)
		{
			FocusedInteractable?.OnFocusEnd.Invoke(this);
			newInteractable?.OnFocusStart.Invoke(this);
		}
		FocusedInteractable = newInteractable;
		if (FocusedInteractable)
		{
			FocusCube.position = FocusedInteractable.Bounds.center;
			FocusCube.localScale = transform.worldToLocalMatrix.MultiplyVector(FocusedInteractable.Bounds.size);
			FocusedInteractable.OnFocus.Invoke(this);
			if (m_useAction.triggered)
			{
				FocusedInteractable.OnUsed.Invoke(this);
			}
		}
		else
		{
			FocusCube.position = CameraController.FocusPoint;
			FocusCube.localScale = FocusCubeSize * Vector3.one;
		}
	}
}
