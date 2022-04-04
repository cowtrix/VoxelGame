using System.Collections;
using UnityEngine;
using Voxul;
using Voxul.Utilities;

namespace Interaction.Activities
{
	public class ConveyorForge : ConveyorBelt
	{
		public Light Light;
		public VoxelColorTint Tint;
		[ColorUsage(false, true)]
		public Color HeatColor;
		public Vector3 DisposalPoint;
		private float m_heat;
		public bool IsOn => CurrentItem;

		private void Start()
		{
			Tint.Color = Color.Lerp(Color.black, HeatColor, m_heat);
			Tint.Invalidate();
			Light.intensity = 0;
		}

		private void Update()
		{
			var newHeat = Mathf.Clamp01(m_heat + (IsOn ? 1 : -1) * Time.deltaTime);
			if (newHeat == m_heat)
			{
				return;
			}
			m_heat = newHeat;
			Light.intensity = Mathf.Lerp(0, 5, m_heat);
			Tint.Color = Color.Lerp(Color.black, HeatColor, m_heat);
			Tint.Invalidate();
		}

		protected override void OnHold(float normalizedHoldTime)
		{
			CurrentItem.TransitionAmount = normalizedHoldTime;
			base.OnHold(normalizedHoldTime);
		}

		protected override IEnumerator ReceiveItem(ForgeResource rsc, ConveyorBelt next)
		{
			var baseCoroutine = base.ReceiveItem(rsc, next);
			while (baseCoroutine.MoveNext())
			{
				yield return baseCoroutine.Current;
			}
			var wDisposalPoint = transform.localToWorldMatrix.MultiplyPoint3x4(DisposalPoint);
			var flatPoint = wDisposalPoint.Flatten(CurrentItem.transform.position.y);
			while (CurrentItem.transform.position != flatPoint)
			{
				CurrentItem.transform.position = Vector3.MoveTowards(CurrentItem.transform.position, flatPoint, MoveSpeed * Time.deltaTime);
				yield return null;
			}
			while (CurrentItem.transform.position != wDisposalPoint)
			{
				CurrentItem.transform.position = Vector3.MoveTowards(CurrentItem.transform.position, wDisposalPoint, MoveSpeed * Time.deltaTime);
				yield return null;
			}
			Destroy(CurrentItem.gameObject);
			CurrentItem = null;
		}

		protected override void OnDrawGizmosSelected()
		{
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.DrawWireCube(DisposalPoint, .1f * Vector3.one);
			base.OnDrawGizmosSelected();
		}
	}
}