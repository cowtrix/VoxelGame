using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Voxul.Utilities;

public class HUDManager : Singleton<HUDManager>
{
	public Image Icon;
	public Text ActionLabel;

	private Camera Camera => CameraController.GetComponent<Camera>();
	public CameraController CameraController => CameraController.Instance;
	public PlayerActor PlayerActor;
	public Material InteractionMaterial;

	public MeshRenderer InteractionObjectRenderer;
	public MeshFilter InteractionObjectFilter;

	public StringEvent FocuseInteractableDisplayName;

	private void Start()
	{
		InteractionObjectRenderer = new GameObject("InteractionRenderer")
			.AddComponent<MeshRenderer>();
		InteractionObjectRenderer.sharedMaterial = InteractionMaterial;
		InteractionObjectFilter = InteractionObjectRenderer.gameObject.AddComponent<MeshFilter>();
	}

	private void Update()
	{
		var interactable = PlayerActor.FocusedInteractable;
		if (interactable)
		{
			InteractionObjectFilter.gameObject.SetActive(true);
			InteractionObjectFilter.sharedMesh = interactable.GetInteractionMesh();
			InteractionObjectFilter.transform.position = interactable.transform.position;
			InteractionObjectFilter.transform.rotation = interactable.transform.rotation;
			InteractionObjectFilter.transform.localScale = interactable.transform.lossyScale;
			Icon.sprite = PlayerActor.FocusedInteractable.InteractionSettings.Icon?.Invoke();

			ActionLabel.gameObject.SetActive(true);
			ActionLabel.text = interactable.GetActions(PlayerActor).FirstOrDefault();

			FocuseInteractableDisplayName.Invoke(interactable.DisplayName);
		}
		else
		{
			Icon.sprite = null;
			ActionLabel.gameObject.SetActive(false);
			InteractionObjectFilter.gameObject.SetActive(false);
			FocuseInteractableDisplayName.Invoke("");
		}
		Icon.gameObject.SetActive(Icon.sprite);
	}
}
