using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Voxul;
using Voxul.Utilities;

public class MainMenu : ExtendedMonoBehaviour
{
	public enum eOpenState
	{
		Closed, Opening, Open, Closing,
	}

	public Color OpenColor, ClosedColor;
	public Sprite TileSprite;
	public CanvasGroup Group;
	public Transform TileContainer;
	public float TileSpeed = 1;

	public eOpenState OpenState { get; private set; }

	public Dictionary<Vector2, Image> Tiles { get; private set; } = new Dictionary<Vector2, Image>();

	private void Start()
	{
		var screenWidth = Screen.width;
		var screenHeight = Screen.height;
		var step = Mathf.FloorToInt(screenWidth / 32f);
		for (var x = 0; x <= screenWidth; x += step)
		{
			for (var y = 0; y <= screenHeight + step; y += step)
			{
				var vec = new Vector2(x, y);
				var newTile = new GameObject($"Tile_{x}_{y}").AddComponent<Image>();
				newTile.GetComponent<RectTransform>().sizeDelta = new Vector2(step, step);
				newTile.transform.SetParent(TileContainer);
				newTile.transform.localPosition = GetDisabledOffset(vec);
				newTile.transform.localScale = default;
				newTile.color = ClosedColor;
				newTile.sprite = TileSprite;
				Tiles[vec] = newTile;
			}
		}
	}

	public Vector2 GetDisabledOffset(Vector2 vec)
	{
		if (vec.magnitude > new Vector2(Screen.width / 2f, Screen.height / 2f).magnitude)
		{
			return vec + Vector2.one * new Vector2(Screen.width, Screen.height).magnitude / 2f;
		}
		return vec - Vector2.one * new Vector2(Screen.width, Screen.height).magnitude / 2f;
	}

	public void Open()
	{
		if (OpenState == eOpenState.Opening || OpenState == eOpenState.Open)
		{
			Debug.Log($"Didn't open because OpenState was {OpenState}");
			return;
		}
		Debug.Log("Opening main menu");
		StartCoroutine(OpenAsync());
	}

	public void Toggle(InputAction.CallbackContext cntxt)
	{
		if (!cntxt.canceled)
		{
			return;
		}
		if (OpenState != eOpenState.Open)
		{
			Open();
		}
		else if (OpenState != eOpenState.Closed)
		{
			Close();
		}
	}

	IEnumerator OpenAsync()
	{
		bool anyUpdated = true;
		OpenState = eOpenState.Opening;
		while (anyUpdated && OpenState == eOpenState.Opening)
		{
			anyUpdated = false;
			foreach (var tile in Tiles)
			{
				var targetPos = tile.Key;
				var tileImg = tile.Value;
				if ((tileImg.transform.position.xy() - targetPos).magnitude < .01f
					&& tileImg.transform.localScale == Vector3.one
					&& Common.ColorExtensions.DistanceBetweenColors(tileImg.color, OpenColor) < .1f)
				{
					continue;
				}
				anyUpdated = true;
				var dt = Time.deltaTime * TileSpeed;
				tileImg.transform.localPosition = VectorExtensions.Berp(tileImg.transform.localPosition, targetPos, dt);
				tileImg.transform.localScale = Vector3.MoveTowards(tileImg.transform.localScale, Vector3.one, dt);
				tileImg.color = Color.Lerp(tileImg.color, OpenColor, dt);
			}
			yield return null;
		}
		OpenState = eOpenState.Open;
		Debug.Log("Finished opening main menu");
	}

	public void Close()
	{
		if (OpenState == eOpenState.Closing || OpenState == eOpenState.Closed)
		{
			Debug.Log($"Didn't close because OpenState was {OpenState}");
			return;
		}
		Debug.Log("Closing main menu");
		StartCoroutine(CloseAsync());
	}

	IEnumerator CloseAsync()
	{
		bool anyUpdated = true;
		OpenState = eOpenState.Closing;
		while (anyUpdated && OpenState == eOpenState.Closing)
		{
			anyUpdated = false;
			foreach (var tile in Tiles)
			{
				var targetPos = GetDisabledOffset(tile.Key);
				var tileImg = tile.Value;
				if ((tileImg.transform.position.xy() - targetPos).magnitude < .1f
					&& tileImg.transform.localScale == Vector3.zero
					&& Common.ColorExtensions.DistanceBetweenColors(tileImg.color, ClosedColor) < .1f)
				{
					continue;
				}
				anyUpdated = true;
				var dt = Time.deltaTime * TileSpeed;
				tileImg.transform.localPosition = Vector3.Lerp(tileImg.transform.localPosition, targetPos, dt);
				tileImg.transform.localScale = Vector3.Lerp(tileImg.transform.localScale, Vector3.zero, dt);
				tileImg.color = Color.Lerp(tileImg.color, ClosedColor, dt * 2);
			}
			yield return null;
		}
		OpenState = eOpenState.Closed;
		Debug.Log("Finished closing main menu");
	}
}
