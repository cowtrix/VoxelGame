using Actors;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Voxul;
using Voxul.Utilities;

namespace Interaction.Activities
{
	public class Forge : Activity
	{
		public override string DisplayName => "Asteroid Forge";
		public bool IsFiring { get; private set; }

		public AsteroidGenerator Asteroid;
		public float AsteroidRotationSpeed = 1;

		public Transform Gun, GunSupport, ResourceSink;
		public float ResourceMoveSpeed = 1;
		public float MaxGunRange = 100;
		public float HitTime = 1;
		public float GunChaseSpeed = 1;
		public Vector3 GunFirePosition;
		public Vector3 AdditionalRotation;
		public RotationLimits GunRotationLimits;
		public LineRenderer Laser;
		public ParticleSystem Sparks;
		public VoxelColorTint MineCube;

		public Canvas Canvas;
		public Image Purity, Remaining;
		public ForgeConveyorManager ConveyorManager;
		public float NextAsteroidHoldTime = 1;
		public Vector3 AsteroidPoint, AsteroidGenerationPoint;
		public float AsteroidMoveSpeed = 1;
		public ParticleSystem GenParticles;

		private Vector2 m_lastMove;
		private float m_currentHitTime;
		private VoxelCoordinate m_currentHit;
		private VoxelColorTint m_hotCube;
		private List<Rigidbody> m_cubes = new List<Rigidbody>();

		private bool m_isRequestingNewAsteroid;
		private float m_newAsteroidHoldTimer;

		public override IEnumerable<ActorAction> GetActions(Actor context)
		{
			if (context == Actor)
			{
				yield return new ActorAction { Key = eActionKey.LOOK, Description = "Aim Laser" };
				yield return new ActorAction { Key = eActionKey.FIRE, Description = "Fire Laser" };
				yield return new ActorAction { Key = eActionKey.MOVE, Description = "Rotate Asteroid" };
				yield return new ActorAction { Key = eActionKey.USE, Description = "Next Asteroid" };
			}
			foreach (var baseAction in base.GetActions(context))
			{
				yield return baseAction;
			}
		}

		protected override void Start()
		{
			MineCube.gameObject.SetActive(false);
			base.Start();
		}

		private void Update()
		{
			if (m_isRequestingNewAsteroid)
			{
				m_newAsteroidHoldTimer += Time.deltaTime;
			}
			else
			{
				m_newAsteroidHoldTimer = 0;
			}
			if(m_newAsteroidHoldTimer > 1)
			{
				StartCoroutine(GenerateNewAsteroid());
			}

			var targetPoint = Asteroid.transform.position;
			if (Actor)
			{
				var hit = Actor.LastRaycast;
				if (hit.HasValue && hit.Value.collider && hit.Value.distance < MaxGunRange && hit.Value.collider.name == "Asteroid")
				{
					targetPoint = hit.Value.point;
					var voxelPoint = VoxelCoordinate.FromVector3(Asteroid.transform.worldToLocalMatrix.MultiplyPoint3x4(targetPoint - hit.Value.normal * .1f), Asteroid.Layer);
					if (voxelPoint != m_currentHit)
					{
						m_currentHitTime = 0;
					}
					m_currentHit = voxelPoint;
					DebugHelper.DrawPoint(targetPoint, .1f, Color.green, 0);
				}
				else
				{
					targetPoint = Actor.LookAdapter.transform.position + Actor.LookAdapter.transform.forward * MaxGunRange;
					DebugHelper.DrawPoint(targetPoint, .1f, Color.red, 0);
				}
				Canvas.gameObject.SetActive(true);
			}
			else
			{
				Canvas.gameObject.SetActive(false);
			}
			Debug.DrawLine(Gun.position, targetPoint);
			var gunDelta = transform.localToWorldMatrix.MultiplyVector(Gun.transform.localToWorldMatrix.MultiplyPoint3x4(GunFirePosition) - targetPoint);
			var rot = Quaternion.Euler(AdditionalRotation) * Quaternion.LookRotation(gunDelta.normalized, transform.up);

			Gun.localRotation = Quaternion.LerpUnclamped(Gun.localRotation, Quaternion.Euler(0, 0, -rot.eulerAngles.x), Time.deltaTime * GunChaseSpeed);
			GunSupport.localRotation = Quaternion.LerpUnclamped(GunSupport.localRotation, Quaternion.Euler(0, rot.eulerAngles.y, 0), Time.deltaTime * GunChaseSpeed);

			var asteroidRot = m_lastMove * Time.deltaTime * AsteroidRotationSpeed;
			Asteroid.transform.Rotate(0, asteroidRot.x, -asteroidRot.y);

			for (int i = m_cubes.Count - 1; i >= 0; i--)
			{
				var cube = m_cubes[i];
				var delta = ResourceSink.position - cube.transform.position;
				var distance = delta.magnitude;
				const float minDist = 2;
				if (distance > minDist && distance < MaxGunRange)
				{
					cube.AddForce(delta.normalized * Time.deltaTime * ResourceMoveSpeed * (delta.magnitude > minDist ? 1 : delta.magnitude / minDist), ForceMode.Impulse);
				}
				else
				{
					Destroy(cube.gameObject);
					m_cubes.RemoveAt(i);
					ConveyorManager.StackCount++;
				}
			}

			if (IsFiring)
			{
				Laser.enabled = true;
				Laser.useWorldSpace = true;
				Laser.SetPosition(0, Gun.transform.localToWorldMatrix.MultiplyPoint3x4(GunFirePosition));
				Laser.SetPosition(1, targetPoint);
				Sparks.transform.position = targetPoint;
				Sparks.Play();

				m_currentHitTime += Time.deltaTime;
				if (!m_hotCube)
				{
					m_hotCube = Instantiate(MineCube.gameObject).GetComponent<VoxelColorTint>();
					m_hotCube.gameObject.SetActive(true);
					m_hotCube.transform.SetParent(Asteroid.transform);
					m_hotCube.transform.localRotation = Quaternion.identity;
					m_hotCube.transform.localPosition = m_currentHit.ToVector3();
				}
				if (Asteroid.Renderer.Mesh.Voxels.TryGetValue(m_currentHit, out var vox))
				{
					m_hotCube.Color = Color.Lerp(vox.Material.Default.Albedo, vox.Material.Default.Albedo * 2, m_currentHitTime / HitTime);
					m_hotCube.Invalidate();
					if (m_currentHitTime > HitTime)
					{
						m_currentHitTime = 0;
						Asteroid.Renderer.Mesh.Voxels.Remove(m_currentHit);
						Asteroid.Renderer.Mesh.Invalidate();
						Asteroid.Renderer.Invalidate(true, true);

						var coll = m_hotCube.gameObject.AddComponent<SphereCollider>();
						coll.radius = .04f;
						m_cubes.Add(m_hotCube.gameObject.AddComponent<Rigidbody>());
						m_hotCube = null;
					}
				}
			}
			else
			{
				Laser.enabled = false;
				Sparks.Stop();
				m_currentHitTime = 0;
				if (m_hotCube)
				{
					Destroy(m_hotCube.gameObject);
					m_hotCube = null;
				}
			}
		}

		public override void ReceiveAction(Actor actor, ActorAction action)
		{
			if (Actor == actor)
			{
				switch (action.Key)
				{
					case eActionKey.MOVE:
						m_lastMove = action.Context;
						break;
					case eActionKey.FIRE:
						IsFiring = action.State != eActionState.End;
						break;
					case eActionKey.USE:
						m_isRequestingNewAsteroid = action.State != eActionState.End;
						break;
				}
			}
			base.ReceiveAction(actor, action);
		}

		public override void OnStopActivity(Actor actor)
		{
			m_lastMove = default;
			base.OnStopActivity(actor);
		}

		IEnumerator GenerateNewAsteroid()
		{
			var asteroidPoint = transform.localToWorldMatrix.MultiplyPoint3x4(AsteroidPoint);
			var generationPoint = transform.localToWorldMatrix.MultiplyPoint3x4(AsteroidGenerationPoint);
			while (Asteroid.transform.position != generationPoint)
			{
				Asteroid.transform.position = Vector3.MoveTowards(Asteroid.transform.position, generationPoint, AsteroidMoveSpeed * Time.deltaTime);
				yield return null;
			}
			GenParticles.time = 0;
			GenParticles.Play();
			yield return new WaitForSeconds(.5f);
			Asteroid.Generate();
			while (Asteroid.transform.position != asteroidPoint)
			{
				Asteroid.transform.position = Vector3.MoveTowards(Asteroid.transform.position, asteroidPoint, AsteroidMoveSpeed * Time.deltaTime);
				yield return null;
			}
		}

		protected override void OnDrawGizmosSelected()
		{
			base.OnDrawGizmosSelected();
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.DrawWireCube(GunFirePosition, Vector3.one);
			Gizmos.DrawWireCube(AsteroidGenerationPoint, Vector3.one * .2f);
			Gizmos.DrawWireCube(AsteroidPoint, Vector3.one);
		}
	}
}