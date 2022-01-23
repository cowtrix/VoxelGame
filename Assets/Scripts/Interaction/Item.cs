using Actors;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Voxul.Utilities;

namespace Interaction.Items
{
	public interface IItem
	{
		Transform transform { get; }
		GameObject gameObject { get; }
	}

	public interface IEquippableItem : IItem
	{
		void OnEquip(Actor actor);
		void OnUnequip(Actor actor);
		void UseOn(Actor playerInteractionManager, GameObject target);
	}

	public interface IPurchaseableItem : IItem
	{
		public int Cost { get; }
	}

	public class Item : Interactable, IItem
	{
		[Serializable]
		public struct IconParameters
		{
			public Texture2D Texture;
			public Vector3 Offset, FocusOffset;
		}

		public const int IconSize = 64;
		public const string PICK_UP = "Pick Up";
		public const string DROP = "Drop";

		public string ItemName = "Unknown Item";
		public string Description = "An object of mysterious origins.";
		public Vector3 EquippedOffset, EquippedRotation;
		public IconParameters Icon;
		public bool EquipOnPickup;

		private int m_layer;
		private bool m_isKinematic;

		protected Rigidbody Rigidbody => GetComponent<Rigidbody>();

		public override string DisplayName => ItemName;

		private void Start()
		{
			m_layer = gameObject.layer;
			var rb = Rigidbody;
			if (rb)
			{
				m_isKinematic = rb.isKinematic;
			}
		}

		[ContextMenu("Generate Icon")]
		public void GenerateIcon()
		{
			var layer = gameObject.layer;
			gameObject.layer = 31;
			var rt = RenderTexture.GetTemporary(IconSize, IconSize, 16, RenderTextureFormat.ARGB32);
			var cam = new GameObject().AddComponent<Camera>();
			cam.targetTexture = rt;
			cam.transform.position = transform.position + transform.localToWorldMatrix.MultiplyVector(Icon.Offset);
			cam.transform.LookAt(transform.position + transform.localToWorldMatrix.MultiplyVector(Icon.FocusOffset));
			cam.cullingMask = 1 << 31;
			cam.clearFlags = CameraClearFlags.SolidColor;
			cam.backgroundColor = Color.clear;
			cam.nearClipPlane = .01f;
			var ambientSettings = AmbientLightingSettings.Current;
			new AmbientLightingSettings
			{
				ambientMode = UnityEngine.Rendering.AmbientMode.Flat,
				ambientSkyColor = new Color(.8f, .8f, .8f),
			}.Apply();

			cam.Render();
			RenderTexture.active = rt;
			var tex = new Texture2D(IconSize, IconSize);
			tex.ReadPixels(new Rect(0, 0, IconSize, IconSize), 0, 0);
			tex.Apply();
			RenderTexture.active = null;

			File.WriteAllBytes($"Assets/Prefabs/Items/Resources/Icons/{name}_icon.png", tex.EncodeToPNG());
#if UNITY_EDITOR
			UnityEditor.AssetDatabase.Refresh();
#endif
			Icon.Texture = Resources.Load<Texture2D>($"Icons/{name}_icon");

			cam.targetTexture = null;
			cam.gameObject.SafeDestroy();
			gameObject.layer = layer;
			RenderTexture.ReleaseTemporary(rt);
			this.TrySetDirty();
			ambientSettings.Apply();
		}

		public override IEnumerable<string> GetActions(Actor actor)
		{
			if (!CanUse(actor))
			{
				yield break;
			}
			yield return PICK_UP;
		}

		public override void Use(Actor actor, string action)
		{
			if (action == PICK_UP)
			{
				actor.State.PickupItem(this);
			}
			base.Use(actor, action);
		}

		public virtual void OnPickup(Actor actor)
		{
			var rb = Rigidbody;
			if (rb)
			{
				rb.isKinematic = true;
				rb.detectCollisions = false;
			}
		}

		public virtual void OnDrop(Actor actor)
		{
			var rb = Rigidbody;
			if (rb)
			{
				rb.position = transform.position;
				rb.rotation = transform.rotation;
				rb.isKinematic = m_isKinematic;
				rb.detectCollisions = true;
			}
			gameObject.layer = m_layer;
			transform.SetParent(null, true);
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.matrix = transform.localToWorldMatrix;
			var origin = Icon.Offset;
			Gizmos.DrawCube(origin, Vector3.one * .1f);
			Gizmos.DrawLine(origin, Icon.FocusOffset);
		}
	}
}