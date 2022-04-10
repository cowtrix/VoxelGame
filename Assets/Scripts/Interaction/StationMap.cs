using Actors;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxul;

namespace Interaction
{
	public class StationMap : ExtendedMonoBehaviour
	{
		[Serializable]
		public class Location
		{
			public VoxelColorTint Tint;
		}

		public float Ramp { get; private set; } = 0;
		public VoxelColorTint FocusedTint { get; private set; }
		public bool IsTransporting { get; private set; }

		[ColorUsage(true, true)]
		public Color DefaultColor;
		[ColorUsage(true, true)]
		public Color HighlightedColor;
		public float FadeSpeed = 1;
		public List<Location> Locations;
		public VoxelColorTint CentralColumn;

		public Transform TransportQuad;
		public ParticleSystem TransportParticles;
		public float ParticleGrowSpeed = 1;
		public float TransportSpeed = 1;

		private void Update()
		{
			Ramp = Mathf.Clamp01(Ramp + Time.deltaTime * FadeSpeed * (IsTransporting ? -1 : 1));
			CentralColumn.Color = Color.Lerp(Color.clear, DefaultColor, Ramp);
			CentralColumn.Invalidate();
			foreach (var location in Locations)
			{
				location.Tint.Color = Color.Lerp(Color.clear, !IsTransporting && location.Tint == FocusedTint ? HighlightedColor : DefaultColor, Ramp);
				location.Tint.Invalidate();
			}
		}

		private void OnEnable()
		{
			Ramp = 0;
			foreach (var loc in Locations)
			{
				loc.Tint.GetComponent<SimpleInteractable>().enabled = true;
			}
		}

		public void OnLocationFocused(Transform t)
		{
			if (IsTransporting)
			{
				return;
			}
			FocusedTint = t.GetComponent<VoxelColorTint>();
		}

		public void OnLocationUnfocused(Transform t)
		{
			if (IsTransporting)
			{
				return;
			}
			if (FocusedTint == t.GetComponent<VoxelColorTint>())
			{
				FocusedTint = null;
			}
		}

		public void OnLocationUsed(Transform t)
		{
			if (IsTransporting)
			{
				return;
			}
			SelectLocation(Locations.FindIndex(0, l => l.Tint.transform == t));
		}

		public void SelectLocation(int index)
		{
			IsTransporting = true;
			TransportParticles.gameObject.SetActive(true);
			TransportParticles.Play();
			foreach(var loc in Locations)
			{
				loc.Tint.GetComponent<SimpleInteractable>().enabled = false;
			}
			StartCoroutine(Transport(Locations[index]));
		}

		IEnumerator Transport(Location location)
		{
			FocusedTint = location.Tint;
			var cam = CameraController.Instance;
			var targetParticleScale = Vector3.one * 6;
			while (TransportParticles.transform.localScale.x < 6 || TransportQuad.localScale.x < 6 || TransportQuad.transform.position != cam.transform.position + cam.transform.forward * .5f)
			{
				TransportParticles.transform.localScale = Vector3.MoveTowards(TransportParticles.transform.localScale, Vector3.one * 6, ParticleGrowSpeed * Time.deltaTime);
				TransportQuad.transform.localScale = Vector3.MoveTowards(TransportQuad.transform.localScale, targetParticleScale, TransportSpeed * Time.deltaTime);
				TransportQuad.transform.position = Vector3.MoveTowards(TransportQuad.transform.position, cam.transform.position + cam.transform.forward * .5f, TransportSpeed * Time.deltaTime);
				yield return null;
			}

		}
	}
}