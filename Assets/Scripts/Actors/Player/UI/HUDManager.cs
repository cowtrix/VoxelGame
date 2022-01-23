using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Voxul.Utilities;

public class HUDManager : Singleton<HUDManager>
{
	public Image Icon;
	public UIActionLabel ActionLabel;

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
		var interactable = PlayerActor.FocusedInteractable;
		if (interactable)
		{
			if(interactable != CameraController.Proxy)
			{
				ShowHoverObect(interactable);
			}
			else
			{
				HideHoverObject();
			}

			int actionIndex = 0;
			foreach(var action in interactable.GetActions(PlayerActor))
			{
				UIActionLabel label;
				if(m_labels.Count <= actionIndex)
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
				label.ActionName.text = action;
				actionIndex++;
			}
			for(var i = actionIndex; i < m_labels.Count; ++i)
			{
				m_labels[i].gameObject.SetActive(false);
			}

			FocusedInteractableDisplayName.Invoke(interactable.DisplayName);
		}
		else
		{
			Icon.sprite = null;
			foreach(var label in m_labels)
			{
				label.gameObject.SetActive(false);
			}
			HideHoverObject();
			FocusedInteractableDisplayName.Invoke("");
		}
		Icon.gameObject.SetActive(Icon.sprite);
	}

	private void ShowHoverObect(Interactable interactable)
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
	}
}
