using Actors;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul;
using Voxul.Utilities;

namespace Interaction.Activities
{
	public class Skeexball : Activity
	{
		public int AmmoCount { get; set; }
		public int CurrentScore { get; set; }
		public override string DisplayName => "Skeexball";
		public bool IsPlaying { get; private set; }
		public float FireStrength { get; private set; }
		public bool FirePressed { get; private set; }
		public List<Rigidbody> Rigidbodies { get; private set; } = new List<Rigidbody>();
		public List<SkeexballTarget> Targets { get; private set; } = new List<SkeexballTarget>();

		public SkeexballTarget TargetPrefab;
		public Rigidbody BallPrefab;
		public Bounds GameBounds, TargetSpawnBounds;
		public Vector3 BallGravity;
		public int MaxAmmo = 3;
		public int PlayCost = 1;
		public float TargetSpawnSpeed = 1;

		[Header("Gun")]
		public Transform Gun, GunCharge;
		public VoxelColorTint[] AmmoCounters;
		public float GunSensitivity = 1;
		public float GunStrengthChargeSpeed = 1;
		public float GunForce = 1;
		public Vector3 GunLaunchPosition;
		public RotationLimits GunRotationLimits;

		private float m_nextTarget;

		protected override void Start()
		{
			BallPrefab.gameObject.SetActive(false);
			TargetPrefab.gameObject.SetActive(false);
			base.Start();
		}

		private void Update()
		{
			Gun.localRotation = Quaternion.Euler(new Vector3(-LookAngle.y, LookAngle.x));
			if (IsPlaying)
			{
				if (FirePressed)
				{
					FireStrength = Mathf.Clamp01(FireStrength + GunStrengthChargeSpeed * Time.deltaTime);
				}
				else
				{
					FireStrength = 0;
				}
				GunCharge.localScale = new Vector3(1, 1, FireStrength);
				m_nextTarget -= Time.deltaTime;
				if (m_nextTarget <= 0)
				{
					m_nextTarget = TargetSpawnSpeed;
					var spawnPoint = TargetSpawnBounds.min + new Vector3(TargetSpawnBounds.size.x * Random.value, TargetSpawnBounds.size.y * Random.value, TargetSpawnBounds.size.z * Random.value);
					var newTarget = Instantiate(TargetPrefab.gameObject, transform);
					newTarget.transform.localPosition = spawnPoint;
					newTarget.gameObject.SetActive(true);
					Targets.Add(newTarget.GetComponent<SkeexballTarget>());
				}
				if (AmmoCount == 0 && !Rigidbodies.Any(t => t))
				{
					StopPlaying();
				}
			}
			foreach (var rb in Rigidbodies)
			{
				rb.AddForce(transform.localToWorldMatrix.MultiplyVector(BallGravity) * Time.deltaTime);
			}
		}

		public override IEnumerable<ActorAction> GetActions(Actor context)
		{
			if (context == Actor)
			{
				if (!IsPlaying)
				{
					yield return new ActorAction { Key = eActionKey.USE, Description = $"Insert Coin ({PlayCost}¢)" };
				}
				else
				{
					if (FireStrength <= 0)
					{
						yield return new ActorAction { Key = eActionKey.FIRE, Description = "Power Up" };
					}
					else
					{
						yield return new ActorAction { Key = eActionKey.FIRE, Description = "Fire" };
					}
				}
				yield return new ActorAction { Key = eActionKey.LOOK, Description = "Aim Gun" };
			}
			foreach (var a in base.GetActions(context))
			{
				yield return a;
			}
		}

		public override void ReceiveAction(Actor actor, ActorAction action)
		{
			if (actor == Actor)
			{
				
				if (!IsPlaying && action.Key == eActionKey.USE && action.State == eActionState.End && actor.State.TryAdd(eStateKey.Credits, -PlayCost, DisplayName))
				{
					StartPlaying();
				}
				if (IsPlaying && action.Key == eActionKey.FIRE && AmmoCount > 0)
				{
					FirePressed = action.State != eActionState.End;
					if (action.State == eActionState.End)
					{
						Fire();
					}
				}
			}
			base.ReceiveAction(actor, action);
		}

		void Fire()
		{
			var newBall = Instantiate(BallPrefab).GetComponent<Rigidbody>();
			Rigidbodies.Add(newBall);
			newBall.transform.SetParent(transform);
			newBall.transform.position = Gun.transform.localToWorldMatrix.MultiplyPoint3x4(GunLaunchPosition);
			newBall.gameObject.SetActive(true);
			newBall.AddForce(GunForce * FireStrength * Gun.forward);
			newBall.AddTorque(UnityEngine.Random.onUnitSphere);
			AmmoCount--;
			UpdateAmmoDisplay();
		}

		protected override int Tick(float dt)
		{
			for (int i = Rigidbodies.Count - 1; i >= 0; i--)
			{
				var rb = Rigidbodies[i];
				if (!GameBounds.Contains(transform.worldToLocalMatrix.MultiplyPoint3x4(rb.position)))
				{
					rb.gameObject.SafeDestroy();
					Rigidbodies.RemoveAt(i);
				}
			}
			return base.Tick(dt);
		}

		public void UpdateAmmoDisplay()
		{
			for (var i = 0; i < AmmoCounters.Length; ++i)
			{
				var tint = AmmoCounters[i];
				tint.enabled = i < AmmoCount;
				tint.Invalidate();
			}
		}

		protected void StartPlaying()
		{
			if (IsPlaying)
			{
				return;
			}
			IsPlaying = true;
			m_nextTarget = TargetSpawnSpeed;
			CurrentScore = 0;
			AmmoCount = MaxAmmo;
			UpdateAmmoDisplay();
		}

		void StopPlaying()
		{
			if (!IsPlaying)
			{
				return;
			}
			foreach(var target in Targets)
			{
				if (!target)
				{
					continue;
				}
				Destroy(target.gameObject);
			}
			Targets.Clear();
			IsPlaying = false;
		}

		protected override void OnDrawGizmosSelected()
		{
			base.OnDrawGizmosSelected();
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.DrawLine(Vector3.zero, BallGravity);
			Gizmos.DrawWireCube(GameBounds.center, GameBounds.size);
			Gizmos.DrawWireCube(TargetSpawnBounds.center, TargetSpawnBounds.size);

			Gizmos.matrix = Gun.localToWorldMatrix;
			Gizmos.DrawWireCube(GunLaunchPosition, Vector3.one * .05f);
		}
	}
}