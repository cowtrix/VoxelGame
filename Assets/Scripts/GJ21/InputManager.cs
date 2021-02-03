using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputManager : Singleton<InputManager>
{
	public MeshFilter SelectedItemHighlight;

	public LayerMask InteractionMask;
	public Camera Camera;

	public InteractableItem FocusedItem;
	public GameItem HeldItem;

	public Transform TargetTransform;
	public Transform ShopViewTransform, BoxViewTransform;
	public float RotationSpeed = 10;

	public float DoubleClickSwallow = .2f;
	private float m_clickTimer;

	protected void Start()
	{
		TargetTransform = ShopViewTransform;
	}

	public void OnTurnAround()
	{
		Debug.Log("Turning around");
		if (TargetTransform == ShopViewTransform)
		{
			TargetTransform = BoxViewTransform;
		}
		else
		{
			TargetTransform = ShopViewTransform;
			LostFoundGameManager.Instance.Boxes.ForEach(b => b.DelayClose(.5f));
		}
	}

	private void Update()
	{
		transform.rotation = Quaternion.Lerp(transform.rotation, TargetTransform.rotation, Time.deltaTime * RotationSpeed);
		transform.position = Vector3.Lerp(transform.position, TargetTransform.position, Time.deltaTime * RotationSpeed);
		m_clickTimer -= Time.deltaTime;
		if (FocusedItem)
		{
			SelectedItemHighlight.gameObject.SetActive(true);
			SelectedItemHighlight.sharedMesh = FocusedItem.GetComponent<MeshFilter>().sharedMesh;
			SelectedItemHighlight.transform.position = FocusedItem.transform.position;
			SelectedItemHighlight.transform.rotation = FocusedItem.transform.rotation;
			SelectedItemHighlight.transform.localScale = FocusedItem.transform.localScale;
		}
		else
		{
			SelectedItemHighlight.gameObject.SetActive(false);
		}
		UIManager.Instance.VerbText.text = FocusedItem?.Verb;
		if (HeldItem)
		{
			HeldItem.transform.SetParent(UIManager.Instance.HeldItemContainer);
			HeldItem.GetComponent<Rigidbody>().isKinematic = true;
			HeldItem.transform.localPosition = Vector3.zero;
			HeldItem.transform.localRotation = Quaternion.Euler(0, 180, 0);
			HeldItem.gameObject.layer = 5;
			UIManager.Instance.HeldItemName.text = HeldItem.name.Replace("(Clone)", "").Trim();
			UIManager.Instance.HeldItemDescription.text = HeldItem?.Description;
			for (int i = 0; i < HeldItem.Traits.Count; i++)
			{
				var trait = HeldItem.Traits[i];
				var rect = UIManager.Instance.HeldItemTraits[i];
				rect.gameObject.SetActive(true);
				var spr = rect.GetComponentInChildren<Image>();
				var txt = rect.GetComponentInChildren<Text>();
				spr.sprite = LostFoundGameManager.Instance.TraitSprites.Single(s => s.Trait == trait).Sprite;
				txt.text = trait.ToString();
			}
			for (int i = HeldItem.Traits.Count; i < 4; i++)
			{
				UIManager.Instance.HeldItemTraits[i].gameObject.SetActive(false);
			}
		}
		UIManager.Instance.HeldItemContainer.gameObject.SetActive(HeldItem);
	}

	public void OnMoveMouse(InputAction.CallbackContext info)
	{
		var pos = info.ReadValue<Vector2>();
		UIManager.Instance.Cursor.position = pos;
		FocusedItem = null;
		if (Physics.Raycast(Camera.ScreenPointToRay(pos), out var hit, InteractionMask))
		{
			Debug.Log($"Hit: {hit.collider.name}", hit.collider);
			var dr = hit.collider.GetComponent<InteractableItem>()
				?? hit.collider.GetComponentInParent<InteractableItem>();
			if (dr)
			{
				FocusedItem = dr;
			}
		}
	}

	public void OnClick()
	{
		if(!FocusedItem || m_clickTimer > 0)
		{
			return;
		}
		m_clickTimer = DoubleClickSwallow;
		if (FocusedItem is BoxItem box)
		{
			if(HeldItem && box.Open)
			{
				var go = HeldItem;
				HeldItem = null;
				go.transform.SetParent(null);
				go.gameObject.layer = 9;
				go.transform.position = box.transform.position + box.InsertPosition;
				var rb = go.GetComponent<Rigidbody>();
				rb.isKinematic = false;
				//box.StoredItems.Add(go);
				box.DelayClose(.5f);
				TargetTransform = BoxViewTransform;
			}
			else
			{
				LostFoundGameManager.Instance.Boxes.ForEach(b => { if (b != box) b.DelayClose(.5f); });
				box.SetOpen(!box.Open);
				if (box.Open)
				{
					TargetTransform = box.CameraTransform;
				}
				else
				{
					TargetTransform = BoxViewTransform;
				}
			}
		}
		else if (FocusedItem is GameItem item && HeldItem == null)
		{
			HeldItem = item;
			if(HeldItem == LostFoundGameManager.Instance.Mat.CurrentItem)
			{
				LostFoundGameManager.Instance.Mat.CurrentItem = null;
			}
			if(TargetTransform.parent?.GetComponent<BoxItem>())
			{
				TargetTransform = BoxViewTransform;
			}
			LostFoundGameManager.Instance.Boxes.ForEach(b =>
			{
				b.DelayClose(.5f);
			});
		}
		else if(FocusedItem is PlacementMat mat)
		{
			if(HeldItem && !mat.CurrentItem)
			{
				var go = HeldItem;
				HeldItem = null;
				go.gameObject.layer = 9;
				mat.CurrentItem = go;
				go.transform.SetParent(null);
				go.transform.position = mat.transform.position + mat.InsertPosition;
				var rb = go.GetComponent<Rigidbody>();
				rb.isKinematic = false;
			}
		}
		else if(HeldItem && FocusedItem is BinItem bin)
		{
			Destroy(HeldItem.gameObject);
			bin.Destr();
			HeldItem = null;
		}
		else if (FocusedItem is Credits cr)
		{
			cr.CreditsContainer.SetActive(!cr.CreditsContainer.activeSelf);
		}
	}
}
