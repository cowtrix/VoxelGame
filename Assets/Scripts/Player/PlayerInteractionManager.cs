using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInteractionManager : Actor
{
	public CameraController CameraController => CameraController.Instance;
	public Interactable FocusedInteractable { get; private set; }

	private void Update()
	{
		/*var newFocused = Interactables
			.Select(f => (, f))
			.Where(f => f.Item1 > 0)
			.OrderBy(f => f.Item1)
			.FirstOrDefault().Item2;*/
		var cameraForward = CameraController.transform.forward;
		var cameraPos = CameraController.transform.position;
		var bestAngle = float.MaxValue;
		var bestDistance = float.MaxValue;
		Interactable newFocused = null;
		foreach(var interactable in Interactables)
		{
			var diffVec = interactable.transform.position - cameraPos;
			var distance = diffVec.magnitude;
			var angle = Vector3.Angle(diffVec.normalized, cameraForward);
			if (angle < bestAngle && distance < bestDistance)
			{
				bestAngle = angle;
				bestDistance = distance;
				newFocused = interactable;
			}
		}
		if(newFocused == FocusedInteractable)
		{
			return;
		}
		FocusedInteractable?.ExitFocus(this);
		FocusedInteractable = newFocused;
		FocusedInteractable?.EnterFocus(this);
	}

	public void OnFire(InputAction.CallbackContext cntxt)
	{
		if (!cntxt.started)
		{
			return;
		}
		FocusedInteractable?.Use(this, "");
	}
}
