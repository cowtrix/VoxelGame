using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interaction.Activities
{
	public class SkeexballTarget : MonoBehaviour
	{
		public Skeexball Game;
		public ParticleSystem DestroyEffect;

		public int Score = 1;
		public int AmmoBonus = 1;
		public bool MoveEnabled;
		public float MoveSpeed = 1;
		public Vector2 MoveLength = new Vector2(1, 1);

		public Rigidbody Rigidbody => GetComponent<Rigidbody>();

		private void OnCollisionEnter(Collision collision)
		{
			if (!collision.collider.name.StartsWith("Ball"))
			{
				return;
			}
			Game.CurrentScore += Score;
			Game.AmmoCount = Mathf.Min(Game.CurrentScore + AmmoBonus, Game.MaxAmmo);
			Game.UpdateAmmoDisplay();
			DestroyEffect.transform.SetParent(Game.transform, true);
			DestroyEffect.Play();
			Destroy(gameObject);
		}

		private void Start()
		{
			if (!MoveEnabled)
			{
				return;
			}
			StartCoroutine(Bounce());
		}

		IEnumerator Bounce()
		{
			var targetPos = transform.localToWorldMatrix.MultiplyPoint3x4(Vector3.up * Random.Range(MoveLength.x, MoveLength.y));
			var rootPos = transform.position;
			while ((Rigidbody.position - targetPos).magnitude > .1f)
			{
				Rigidbody.MovePosition(Vector3.MoveTowards(Rigidbody.position, targetPos, MoveSpeed * Time.deltaTime));
				yield return null;
			}
			while ((Rigidbody.position - rootPos).magnitude > .1f)
			{
				Rigidbody.MovePosition(Vector3.MoveTowards(Rigidbody.position, rootPos, MoveSpeed * Time.deltaTime));
				yield return null;
			}
			Destroy(gameObject);
		}
	}
}