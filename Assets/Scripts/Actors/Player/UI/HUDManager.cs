using Actors;
using Common;
using Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Voxul.Utilities;

namespace UI
{
	[Serializable]
	public class StringEvent : UnityEvent<string> { }

	public class HUDManager : Singleton<HUDManager>
	{
		public Image Icon;
		public UIActionLabel ActionLabel;
		public RectTransform FocusSprite;

		private Camera Camera => CameraController.GetComponent<Camera>();
		public CameraController CameraController => CameraController.Instance;
		public PlayerActor PlayerActor;
		public Material InteractionMaterial;

		public MeshRenderer InteractionObjectRenderer;
		public MeshFilter InteractionObjectFilter;

		public StringEvent FocusedInteractableDisplayName;

		private List<UIActionLabel> m_labels = new List<UIActionLabel>();

		private void Start()
		{
			InteractionObjectRenderer = new GameObject("InteractionRenderer")
				.AddComponent<MeshRenderer>();
			InteractionObjectRenderer.sharedMaterial = InteractionMaterial;
			InteractionObjectFilter = InteractionObjectRenderer.gameObject.AddComponent<MeshFilter>();

			ActionLabel.gameObject.SetActive(false);
		}

		private void Update()
		{
			var interactable = PlayerActor.FocusedInteractable ?? PlayerActor.State.EquippedItem as Interactable ?? PlayerActor.CurrentActivity;
			if (interactable && interactable != PlayerActor.CurrentActivity)
			{
				FocusSprite.gameObject.SetActive(true);

				var screenRect = new Rect(Camera.WorldToScreenPoint(interactable.transform.position), Vector2.zero);
				var objBounds = interactable.Bounds;
				foreach(var p in objBounds.AllPoints())
				{
					screenRect = screenRect.Encapsulate(Camera.WorldToScreenPoint(p));
				}
				FocusSprite.position = screenRect.center;
				FocusSprite.sizeDelta = screenRect.size;

				FocusedInteractableDisplayName.Invoke(interactable.DisplayName);
			}
			else
			{
				FocusSprite.gameObject.SetActive(false);
				FocusedInteractableDisplayName.Invoke("");
			}

			if (interactable && interactable.CanUse(PlayerActor))
			{
				int actionIndex = 0;
				foreach (var action in interactable.GetActions(PlayerActor))
				{
					UIActionLabel label;
					if (m_labels.Count <= actionIndex)
					{
						label = Instantiate(ActionLabel.gameObject).GetComponent<UIActionLabel>();
						label.transform.SetParent(ActionLabel.transform.parent);
						m_labels.Add(label);
					}
					else
					{
						label = m_labels[actionIndex];
					}
					label.gameObject.SetActive(true);
					label.ActionIcon.sprite = null;
					label.ActionName.text = action.ToString();
					actionIndex++;
				}
				for (var i = actionIndex; i < m_labels.Count; ++i)
				{
					m_labels[i].gameObject.SetActive(false);
				}

			}
			else
			{
				Icon.sprite = null;
				foreach (var label in m_labels)
				{
					label.gameObject.SetActive(false);
				}
				//HideHoverObject();
			}
			Icon.gameObject.SetActive(Icon.sprite);
		}

		/*private void ShowHoverObect(Interactable interactable)
		{
			InteractionObjectFilter.gameObject.SetActive(true);
			InteractionObjectFilter.sharedMesh = interactable.GetInteractionMesh();
			InteractionObjectFilter.transform.position = interactable.transform.position;
			InteractionObjectFilter.transform.rotation = interactable.transform.rotation;
			InteractionObjectFilter.transform.localScale = interactable.transform.lossyScale;
			Icon.sprite = PlayerActor.FocusedInteractable.InteractionSettings.Icon?.Invoke();
		}

		private void HideHoverObject()
		{
			InteractionObjectFilter.gameObject.SetActive(false);
		}*/
	}
}