using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class RaycastHitEvent : UnityEvent<RaycastHit> { }

namespace Weapons
{
	[Serializable]
	public struct ImpactEffect
	{
		public enum ESpawnMode
		{
			Always,
			DestroyableOnly,
			NonDestroyableOnly,
		}

		public GameObject Object;
		public ESpawnMode SpawnMode;
		public bool AlignToNormal, Flip;
		public Vector3 HitOffset;

		public void Hit(RaycastHit hit)
		{
			if (!Object)
			{
				return;
			}
			switch(SpawnMode)
			{
				case ESpawnMode.NonDestroyableOnly:
					if (hit.collider.GetComponent<DestroyableVoxel>())
						return;
					break;
				case ESpawnMode.DestroyableOnly:
					if (!hit.collider.GetComponent<DestroyableVoxel>())
						return;
					break;
			}
			var eff = UnityEngine.Object.Instantiate(Object);
			eff.SetActive(true);
			eff.transform.position = hit.point;
			if(AlignToNormal)
			{
				eff.transform.LookAt(hit.point + hit.normal * (Flip ? 1 : -1));
			}
			eff.transform.position += eff.transform.localToWorldMatrix.MultiplyVector(HitOffset);
		}
	}

	public abstract class Weapon : MonoBehaviour
	{
		public List<ImpactEffect> ImpactEffects;
		protected CameraController CameraController => CameraController.Instance;
		public float CastDistance = 1000;
		public LayerMask LayerMask;

		public UnityEvent OnWeaponFire;
		public RaycastHitEvent OnWeaponHit;

		private void Start()
		{
			foreach(var eff in ImpactEffects)
			{
				eff.Object?.SetActive(false);
			}
		}

		public virtual void Fire()
		{
			var ray = new Ray(CameraController.transform.position, CameraController.transform.forward);
			if (Physics.Raycast(ray, out var hit, CastDistance, LayerMask, QueryTriggerInteraction.Ignore))
			{
				Debug.DrawLine(ray.origin, hit.point, Color.green);
				OnHit(hit);
				OnWeaponHit.Invoke(hit);
				foreach(var eff in ImpactEffects)
				{
					eff.Hit(hit);
				}
			}
			else
			{
				Debug.DrawRay(ray.origin, ray.direction * CastDistance, Color.red);
			}
			OnWeaponFire.Invoke();
		}

		protected abstract void OnHit(RaycastHit hit);

		public void OnUnequip()
		{
			
		}

		public void OnEquip()
		{
			
		}
	}
}