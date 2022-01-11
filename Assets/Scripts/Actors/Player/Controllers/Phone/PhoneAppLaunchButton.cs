using System;
using UnityEngine.UI;
using Voxul;

public class PhoneAppLaunchButton : ExtendedMonoBehaviour
{
	public PhoneApp App { get; private set; }
	public Button Button => GetComponent<Button>();
	public Text AppLabel;
	public Image AppIcon;
	public Text NotificationCountText;

	public void SetApp(PhoneController phoneController, PhoneApp app)
	{
		App = app;
		AppIcon.sprite = app.Icon;
		AppLabel.text = app.AppName;
		Button.onClick.RemoveAllListeners();
		Button.onClick.AddListener(() => phoneController.OpenApp(app));
	}

	private void Update()
	{
		NotificationCountText.transform.parent.gameObject.SetActive(App.NotificationCount > 0);
		NotificationCountText.text = App.NotificationCount.ToString();
	}
}
