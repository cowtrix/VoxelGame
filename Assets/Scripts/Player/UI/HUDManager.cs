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
	public PlayerInteractionManager InteractionManager;
	public Material InteractionMaterial;

	public MeshRenderer InteractionObjectRenderer;
	public MeshFilter InteractionObjectFilter;

	private void Start()
	{
		InteractionObjectRenderer = new GameObject("InteractionRenderer")
			.AddComponent<MeshRenderer>();
		InteractionObjectRenderer.sharedMaterial = InteractionMaterial;
		InteractionObjectFilter = InteractionObjectRenderer.gameObject.AddComponent<MeshFilter>();
	}

	private void Update()
	{
		var interactable = InteractionManager.FocusedInteractable;
		if (interactable)
		{
			InteractionObjectFilter.gameObject.SetActive(true);
			InteractionObjectFilter.sharedMesh = interactable.GetInteractionMesh();
			InteractionObjectFilter.transform.position = interactable.transform.position;
			InteractionObjectFilter.transform.rotation = interactable.transform.rotation;
			InteractionObjectFilter.transform.localScale = interactable.transform.lossyScale;
			Icon.sprite = InteractionManager.FocusedInteractable.InteractionSettings.Icon?.Invoke();

			ActionLabel.gameObject.SetActive(true);
			ActionLabel.text = interactable.GetActions().FirstOrDefault();
		}
		else
		{
			Icon.sprite = null;
			ActionLabel.gameObject.SetActive(false);
			InteractionObjectFilter.gameObject.SetActive(false);
		}
		Icon.gameObject.SetActive(Icon.sprite);
	}
}
