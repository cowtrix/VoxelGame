using UnityEngine;

public class WaveInterction : PeripheralInteraction
{
	protected override void Awake()
	{
		base.Awake();
		Interactable.OnUsed.AddListener(p =>
		{
			Debug.Log($"Waved at {name}");
		});
		Interactable.Icon = () => UIResources.Instance.WaveIcon;
	}
}
