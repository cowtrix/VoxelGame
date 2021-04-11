using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Weapons;

public class PlayerInteractionManager : MonoBehaviour
{
	public Weapon CurrentWeapon;

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

	public CameraController CameraController => CameraController.Instance;
	public Interactable FocusedInteractable;
	private RaycastHit? m_currentHit;

	public void OnFire(InputAction.CallbackContext cntxt)
	{
		if (!cntxt.started)
		{
			return;
		}
		CurrentWeapon?.Fire();
	}
}
