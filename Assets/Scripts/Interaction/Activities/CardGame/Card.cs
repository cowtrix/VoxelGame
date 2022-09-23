using vSplines;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Voxul;
using Voxul.Utilities;

namespace Interaction.Activities.CardGame_Babo
{
	public class Card : ExtendedMonoBehaviour
	{
		public BaboCardGame Game;
		public ECardType CardType;
		public Text CardText;

		public float Curviness = .5f;

		public DynamicTextureSetter FaceTextureSetter;
		public SimpleInteractable Interactable => GetComponent<SimpleInteractable>();

		public Vector3 TargetNormal { get; set; }
		public Vector3 TargetPosition { get; set; }
		public float MoveSpeed = 1;
		private SplineSegment m_moveSpline;
		private float m_splineTime;

		public Quaternion TargetRotation { get; set; }
		public float RotationSpeed = 1;

		public bool Activated { get; set; }
		public GameObject ActivatedContainer;

		public int Cost => GetCost(CardType);

		public void Invalidate()
		{
			FaceTextureSetter.Albedo = Game.GetCardTexture(CardType);
		}

		public static int GetCost(ECardType type)
		{
			switch (type)
			{
				case ECardType.Num_1:
				case ECardType.Num_2:
				case ECardType.Num_3:
				case ECardType.Num_4:
				case ECardType.Num_5:
				case ECardType.Num_6:
					return (int)type + 1;
				case ECardType.LookSelf:
				case ECardType.LookOther:
				case ECardType.Swap:
					return 7;
				case ECardType.LookSwap:
					return -1;
			}
			return 0;
		}

		public static string GetLabel(ECardType type)
		{
			var cost = GetCost(type);
			switch (type)
			{
				case ECardType.Num_1:
				case ECardType.Num_2:
				case ECardType.Num_3:
				case ECardType.Num_4:
				case ECardType.Num_5:
				case ECardType.Num_6:
					return cost.ToString();
				case ECardType.LookSelf:
					return $"Look Self ({cost})";
				case ECardType.LookOther:
					return $"Look Other ({cost})";
				case ECardType.Swap:
					return $"Swap ({cost})";
				case ECardType.LookSwap:
					return $"Look & Swap ({cost})";
			}
			return "";
		}

		private void Update()
		{
			Interactable.Name = "";
			CardText.text = GetLabel(CardType);
			if ((transform.localPosition - TargetPosition).magnitude > 0)
			{
				if(m_moveSpline == null || m_moveSpline.SecondControlPoint.Position != TargetPosition)
				{
					m_moveSpline = new SplineSegment(
						new SplineSegment.ControlPoint(transform.localPosition, -transform.LocalForward() * Curviness, Vector3.zero, Vector3.up),
						new SplineSegment.ControlPoint(TargetPosition, TargetNormal * Curviness, Vector3.zero, Vector3.up),
						1);
					m_splineTime = 0;
				}
			}

			m_splineTime = Mathf.Clamp01(m_splineTime + Time.deltaTime * MoveSpeed);
			transform.localPosition = m_moveSpline.GetNaturalPointOnSplineSegment(m_splineTime);
			transform.localRotation = Quaternion.Lerp(transform.localRotation, TargetRotation, Time.deltaTime * RotationSpeed);

			ActivatedContainer.SetActive(Activated && Interactable.enabled);
		}

		public override string ToString() => GetLabel(CardType);
	}
}