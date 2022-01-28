using Actors;
using Common;
using Interaction.Activities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Interaction.Activities
{
	public class Book : EquippableItemBase
	{
		[Serializable]
		public class Page
		{
			public Vector3 OpenRotation, CloseRotation;
			public Text Content, PageNumber;
			public Transform Root;
		}

		public Page FrontCover, BackCover, MidPageFront, MidPageBack;
		public TextAsset Content;
		public float HoldDistance = 1;
		public float MoveSpeed = 1;
		public float OpenSpeed = 1;
		public int PageNumber;

		public override string DisplayName => Content.name;
		public Rigidbody Rigidbody => GetComponent<Rigidbody>();

		private float m_targetOpen = 0;
		private float m_openAmount = 0;

		[SerializeField]
		[Range(0, 1)]
		private float m_pageTurnAmount = 0;

		public int GetStringSize(string s)
		{
			var text = FrontCover.Content;
			var rect = text.GetComponent<RectTransform>();
			TextGenerationSettings settings = FrontCover.Content.GetGenerationSettings(rect.rect.size);
			return (int)LayoutUtility.GetPreferredHeight(rect);
		}

		protected override void Start()
		{
			const int pageHeight = 300;
			var split = Content.text.Split(' ');
			var sb = new StringBuilder();
			foreach (var w in split)
			{
				if (GetStringSize(sb.ToString() + w) > pageHeight)
				{
					m_pages.Add(sb.ToString());
					sb.Clear();
				}
				sb.Append($"{w} ");
			}
			m_pages.Add(sb.ToString());
		}

		private void Update()
		{
			UpdatePages();

			var rb = Rigidbody;
			if (!EquippedActor)
			{
				rb.WakeUp();
				rb.isKinematic = false;
				rb.detectCollisions = true;
				return;
			}

			m_targetOpen = Mathf.Clamp01(m_targetOpen + Time.deltaTime * OpenSpeed);
			rb.isKinematic = true;
			rb.detectCollisions = false;
			rb.Sleep();
			var camera = CameraController.Instance;
			var targetPosition = camera.transform.position + camera.transform.localToWorldMatrix.MultiplyVector(EquippedOffset) + camera.transform.forward * HoldDistance;
			transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * MoveSpeed);
			transform.rotation = Quaternion.LookRotation(camera.transform.position - transform.position, camera.transform.up) * Quaternion.Euler(EquippedRotation);
		}

		List<string> m_pages = new List<string>();

		void UpdatePages()
		{
			FrontCover.Content.text = m_pages.ElementAtOrDefault(PageNumber);
			FrontCover.PageNumber.text = (PageNumber + 1).ToString();
			MidPageFront.Content.text = m_pages.ElementAtOrDefault(PageNumber + 1);
			MidPageFront.PageNumber.text = (PageNumber + 2).ToString();
			MidPageBack.Content.text = m_pages.ElementAtOrDefault(PageNumber + 2);
			MidPageBack.PageNumber.text = (PageNumber + 3).ToString();
			BackCover.Content.text = m_pages.ElementAtOrDefault(PageNumber + 3);
			BackCover.PageNumber.text = (PageNumber + 4).ToString();

			BackCover.Root.localRotation = Quaternion.Lerp(Quaternion.Euler(BackCover.CloseRotation), Quaternion.Euler(BackCover.OpenRotation), m_targetOpen);
			MidPageBack.Root.localRotation = Quaternion.Lerp(Quaternion.Euler(MidPageBack.CloseRotation), Quaternion.Euler(MidPageBack.OpenRotation), m_pageTurnAmount);

			m_pageTurnAmount = Mathf.Clamp(m_pageTurnAmount, 0, m_openAmount);
		}

		public override void OnEquip(Actor actor)
		{
			m_targetOpen = 1;
			base.OnEquip(actor);
		}

		public override void OnUnequip(Actor actor)
		{
			m_targetOpen = 0;
			base.OnUnequip(actor);
		}

		public override IEnumerable<ActorAction> GetActions(Actor actor)
		{
			if(EquippedActor && actor == EquippedActor)
			{
				yield return new ActorAction { Key = eActionKey.NEXT, Description = "Next Page" };
				yield return new ActorAction { Key = eActionKey.PREV, Description = "Previous Page" };
			}
			foreach(var s in base.GetActions(actor))
			{
				yield return s;
			}
		}

		public override void ExecuteAction(Actor actor, ActorAction action)
		{
			if(action.Key == eActionKey.NEXT && PageNumber < m_pages.Count - 2)
			{
				PageNumber += 2;
				return;
			}
			if (action.Key == eActionKey.PREV && PageNumber > 0)
			{
				PageNumber -= 2;
				return;
			}
			base.ExecuteAction(actor, action);
		}
	}
}