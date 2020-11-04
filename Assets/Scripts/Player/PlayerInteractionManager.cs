using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInteractionManager : MonoBehaviour
{
	public float SpotLightRange = 10;
	public Light SpotLight;

	public float InnerLightRange = 1;
	public Light InnerLight;

	public float FocusCubeSize = .1f;
	public Transform FocusCube;
	public LayerMask InteractionLayerMask;
	public CameraController CameraController;
	public Interactable FocusedInteractable;

	private void Update()
	{
		SpotLight.range = SpotLightRange * transform.lossyScale.x;
		InnerLight.range = InnerLightRange * transform.lossyScale.x;

		var coll = Physics.OverlapBox(CameraController.FocusPoint,
			FocusCubeSize * Vector3.one * .5f,
			transform.rotation, InteractionLayerMask, QueryTriggerInteraction.Collide);
		if (coll.Any())
		{
			FocusedInteractable = coll.Select(c => c.GetComponent<Interactable>())
				.Where(c => c).First();
		}
		else
		{
			FocusedInteractable = null;
		}
		if (FocusedInteractable)
		{
			FocusCube.position = FocusedInteractable.Bounds.center;
			FocusCube.localScale = transform.worldToLocalMatrix.MultiplyVector(FocusedInteractable.Bounds.size);
		}
		else
		{
			FocusCube.position = CameraController.FocusPoint;
			FocusCube.localScale = FocusCubeSize * Vector3.one;
		}
	}
}
