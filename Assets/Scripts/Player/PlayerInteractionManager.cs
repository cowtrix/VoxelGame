using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Weapons;

public class PlayerInteractionManager : Singleton<PlayerInteractionManager>
{
	public Weapon CurrentWeapon
	{
		get
		{
			if (!__weapon)
			{
				CurrentWeapon = AllWeapons.First();
			}
			return __weapon;
		}
		set
		{
			if(__weapon)
			{
				__weapon.OnUnequip();
			}
			__weapon = value;
			__weapon?.OnEquip();
		}
	}
	public Weapon[] AllWeapons;
	private Weapon __weapon;

	public CameraController CameraController => CameraController.Instance;

	public override void Awake()
	{
		base.Awake();
		AllWeapons = GetComponentsInChildren<Weapon>();
	}

	private void Update()
	{
		foreach (var w in AllWeapons)
		{
			w.gameObject.SetActive(w == CurrentWeapon);
		}
	}

	public void OnFire(InputAction.CallbackContext cntxt)
	{
		if (!cntxt.started || !WeaponWheel.Instance.KeyReleased)
		{
			return;
		}
		Debug.Log("PlayerInteractionManager:Fire");
		CurrentWeapon.Fire();
	}
}
