using Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WeaponWheel : Singleton<WeaponWheel>
{
	public float FadeSpeed = 10;
	[Range(0, 1)]
	public float ButtonSize = 0.9f;
	public float CursorOffset = 200;
	public float CursorSpeed = 1;
	public float CursorChase = 10;
	public Vector2 CursorPosition;
	public bool KeyReleased;

	public Toggle WeaponButton;
	public Image Highlight;
	public RectTransform HighlightCursor;
	private PlayerInteractionManager PlayerInteractionManager => PlayerInteractionManager.Instance;
	private List<Toggle> m_selectors = new List<Toggle>();
	private Toggle CurrentSelection => m_selectors.Single(v => v.name == PlayerInteractionManager.CurrentWeapon.name);
	private Toggle ClosestSelection => m_selectors.OrderBy(v => Vector2.SignedAngle(v.transform.right, CursorPosition.normalized)).First();
	private Vector3 m_lastDelta;
	public CanvasGroup Group;

	private void Start()
	{
		Group.alpha = 0;

		var input = MovementController.Instance.Input;
		var weaponAction = input.actions.Single(a => a.name == "SelectWeapon");
		weaponAction.started += OnSelectWeapon;
		weaponAction.canceled += OnSelectWeapon;

		gameObject.SetActive(false);

		var tg = gameObject.AddComponent<ToggleGroup>();
		var interval = 1 / (float)(PlayerInteractionManager.AllWeapons.Length);
		for (int i = 0; i < PlayerInteractionManager.AllWeapons.Length; i++)
		{
			var weapon = PlayerInteractionManager.AllWeapons[i];
			var f = (i / (float)(PlayerInteractionManager.AllWeapons.Length));
			var toggle = Instantiate(WeaponButton.gameObject).GetComponent<Toggle>();
			toggle.name = weapon.name;
			toggle.group = tg;
			m_selectors.Add(toggle);
			toggle.transform.SetParent(transform);
			toggle.transform.localPosition = Vector3.zero;

			var img = toggle.GetComponent<Image>();
			var rot = Quaternion.Euler(0, 0, f * Mathf.PI * Mathf.Rad2Deg * 2);
			img.transform.rotation = rot;
			img.fillAmount = interval * ButtonSize;
			img.transform.GetChild(0).GetComponent<Image>().fillAmount = interval * ButtonSize;

			var label = toggle.GetComponentInChildren<Text>();
			label.text = weapon.name;
			var labelDistance = label.transform.localPosition.magnitude;
			label.transform.localPosition = Vector3.zero;
			label.transform.rotation = rot * Quaternion.Euler(0, 0, -90 + interval * .25f * 360);
			label.transform.Translate(Vector3.down * labelDistance, Space.Self);
			label.transform.SetAsLastSibling();

			toggle.onValueChanged.AddListener(b =>
			{
				if (b)
				{
					PlayerInteractionManager.CurrentWeapon = weapon;
				}
			});
		}
		WeaponButton.gameObject.SetActive(false);
		Highlight.fillAmount = interval * ButtonSize;
	}

	private void OnEnable()
	{
		KeyReleased = false;
		Group.alpha = 1;
	}

	private void OnDisable()
	{
		KeyReleased = true;
	}

	public void OnSelectWeapon(InputAction.CallbackContext context)
	{
		var val = context.ReadValue<float>();
		Debug.Log($"OnSelectWeapon: {val}");
		gameObject.SetActive(!context.canceled);
		CameraController.Instance.LockView = gameObject.activeSelf;

		if(context.canceled)
		{
			ClosestSelection.isOn = true;
			KeyReleased = true;
		}
	}

	private void Update()
	{
		Group.alpha = Mathf.MoveTowards(Group.alpha, KeyReleased ? 0 : 1, Time.deltaTime * FadeSpeed);
		Group.interactable = KeyReleased;
		if (KeyReleased)
		{
			gameObject.SetActive(false);
		}
		var ld = CameraController.Instance.LastDelta;
		if (ld.sqrMagnitude > 0)
		{
			m_lastDelta = ld.normalized;
		}
		CursorPosition = m_lastDelta * CursorSpeed;
		CursorPosition = Vector2.ClampMagnitude(CursorPosition, CursorOffset);
		//HighlightCursor.anchoredPosition = Vector2.Lerp(HighlightCursor.anchoredPosition, CursorPosition, CursorChase * Time.deltaTime);

		CurrentSelection.isOn = true;
		Highlight.transform.localRotation = ClosestSelection.transform.localRotation;
		Highlight.transform.localPosition = ClosestSelection.transform.localPosition; 
	}
}
