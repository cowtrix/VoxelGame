using UnityEngine;

public class WaveInterction : PeripheralInteraction
{
	protected override void Awake()
	{
		base.Awake();
		Interactable.InteractionSettings.OnUsed.AddListener(p =>
		{
			Debug.Log($"Waved at {name}");
		});
		Interactable.InteractionSettings.Icon = () => UIResources.Instance.WaveIcon;
	}
}
