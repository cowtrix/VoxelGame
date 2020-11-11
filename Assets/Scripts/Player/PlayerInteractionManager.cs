using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionManager : MonoBehaviour
{
	public float FocusCubeSize = .1f;
	public Transform FocusCube;
	public LayerMask InteractionLayerMask;

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
		var coll = Physics.OverlapBox(CameraController.FocusPoint,
			FocusCubeSize * Vector3.one * .5f,
			transform.rotation, InteractionLayerMask, QueryTriggerInteraction.Collide);
		Interactable newInteractable = null;
		if (coll.Any())
		{
			newInteractable = coll.Select(c => c.GetComponent<Interactable>())
				.Where(c => c).First();
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
