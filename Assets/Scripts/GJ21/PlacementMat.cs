using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlacementMat : InteractableItem
{
	public List<eItemTrait> CurrentTraits;
	public GameObject TraitContainer;
	public List<Image> TraitSprites;
	public GameItem CurrentItem;
	public Vector3 InsertPosition;
	public List<GameObject> Actors;
	public Vector3 ActorTargetPosition, ActorStartPosition;

	public override string Verb => InputManager.Instance.HeldItem ? "Place" : "";

	private void Update()
	{
		if (CurrentTraits == null || !CurrentTraits.Any())
		{
			TraitContainer.SetActive(false);
			return;
		}
		TraitContainer.SetActive(true);
		for (int i = 0; i < CurrentTraits.Count; i++)
		{
			var trait = CurrentTraits[i];
			var spr = TraitSprites[i];
			spr.gameObject.SetActive(true);
			spr.sprite = LostFoundGameManager.Instance.TraitSprites.Single(s => s.Trait == trait).Sprite;
		}
		for (int i = CurrentTraits.Count; i < 4; i++)
		{
			var spr = TraitSprites[i];
			spr.gameObject.SetActive(false);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawWireCube(ActorTargetPosition, Vector3.one * 0.1f);
		Gizmos.DrawWireCube(ActorStartPosition, Vector3.one * 0.1f);
	}
}
