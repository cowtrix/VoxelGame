using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class RaycastHitEvent : UnityEvent<RaycastHit> { }

public abstract class Weapon : MonoBehaviour
{
	public GameObject ImpactEffect;
	protected CameraController CameraController;
	public float CastDistance = 1000;
	public LayerMask LayerMask;

	public UnityEvent OnWeaponFire;
	public RaycastHitEvent OnWeaponHit;

	private void Start()
	{
		CameraController = CameraController.Instance;
	}

	public virtual void Fire()
	{
		var ray = new Ray(CameraController.transform.position, CameraController.transform.forward);
		if (Physics.Raycast(ray, out var hit, CastDistance, LayerMask, QueryTriggerInteraction.Ignore))
		{
			Debug.DrawLine(ray.origin, hit.point, Color.green);
			OnHit(hit);
			OnWeaponHit.Invoke(hit);
			if(ImpactEffect)
			{
				var eff = Instantiate(ImpactEffect);
				eff.SetActive(true);
				eff.transform.position = hit.point;
				eff.transform.LookAt(hit.point + hit.normal);
			}
		}
		else
		{
			Debug.DrawRay(ray.origin, ray.direction * CastDistance, Color.red);
		}
		OnWeaponFire.Invoke();
	}

	protected abstract void OnHit(RaycastHit hit);

}
