using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LostFoundGameManager : Singleton<LostFoundGameManager>
{ 
	[Serializable]
	public struct TraitSpritePair
	{
		public eItemTrait Trait;
		public Sprite Sprite;
	}
	public List<TraitSpritePair> TraitSprites;

	public enum eGameState
	{
		WAITING,
		PLAYING,
	}
	public eGameState State = eGameState.PLAYING;
	public Vector2 NewItemWaitPeriod = new Vector2(5, 10);
	public PlacementMat Mat;
	public Transform ItemSpawnPoint;
	public List<BoxItem> Boxes;
	public List<GameItem> Items;
	public List<GameItem> SpawnedItems;
	public GameItem CoalLump;

	public float ActorSpeed = 1;
	public Vector2 ActorSpawnPeriod = new Vector2(5, 10);

	private void Start()
	{
		foreach (var i in Items)
		{
			i.gameObject.SetActive(false);
		}
		StartCoroutine(SpawnItems());
		StartCoroutine(ChallengePlayer());
	}

	IEnumerator SpawnItems()
	{
		while (State == eGameState.PLAYING)
		{
			yield return new WaitForSeconds(UnityEngine.Random.Range(NewItemWaitPeriod.x, NewItemWaitPeriod.y));
			var newItem = Items.Random();
			var go = Instantiate(newItem.gameObject);
			SpawnedItems.Add(go.GetComponent<GameItem>());
			go.transform.position = ItemSpawnPoint.position;
			go.transform.SetParent(ItemSpawnPoint);
			yield return null;
			go.gameObject.SetActive(true);
		}
	}

	IEnumerator ChallengePlayer()
	{
		yield return new WaitForSeconds(30);
		while (State == eGameState.PLAYING)
		{
			while(!SpawnedItems.Any(x => x))
			{
				yield return new WaitForSeconds(5);
			}

			// Move to store
			var go = Mat.Actors.Random();
			var startPosition = Mat.transform.localToWorldMatrix.MultiplyPoint3x4(Mat.ActorStartPosition);
			go.transform.position = startPosition;
			var target = Mat.transform.localToWorldMatrix.MultiplyPoint3x4(Mat.ActorTargetPosition);
			var targetRot = Quaternion.LookRotation((target.Flatten() - startPosition.Flatten()).normalized);
			go.transform.rotation = targetRot;
			while ((go.transform.position - target).sqrMagnitude > .1f)
			{
				go.transform.position = Vector3.MoveTowards(go.transform.position, target, Time.deltaTime * ActorSpeed);
				Debug.Log("Moving to store");
				yield return null;
			}
			targetRot = Quaternion.LookRotation((Camera.main.transform.position.Flatten() - target.Flatten()).normalized);
			while(Mathf.Abs(Quaternion.Angle(go.transform.rotation, targetRot)) > .5f)
			{
				go.transform.rotation = Quaternion.Lerp(go.transform.rotation, targetRot, Time.deltaTime * ActorSpeed * 3);
				Debug.Log("Looking at player");
				yield return null;
			}

			yield return new WaitForSeconds(1);
			SpawnedItems = SpawnedItems.Where(i => i && i.transform.position.y > -10).ToList();
			var targetItem = SpawnedItems.Random();
			SpawnedItems.Remove(targetItem);
			Mat.CurrentTraits = targetItem.Traits;

			var waitTime = 20f;
			while((!Mat.CurrentItem || Mat.CurrentTraits.Any(t1 => !Mat.CurrentItem.Traits.Contains(t1))) && waitTime > 0)
			{
				waitTime -= Time.deltaTime;
				yield return null;
			}

			yield return new WaitForSeconds(1);
			if (!Mat.CurrentItem || Mat.CurrentTraits.Any(t1 => !Mat.CurrentItem.Traits.Contains(t1)))
			{
				// No good...
			}
			else
			{
				Destroy(Mat.CurrentItem.gameObject);
				Mat.CurrentItem = null;
			}

			Mat.CurrentTraits.Clear();

			// Move back
			target = Mat.transform.localToWorldMatrix.MultiplyPoint3x4(Mat.ActorStartPosition);
			targetRot = Quaternion.LookRotation((go.transform.position.Flatten() + target.Flatten()).normalized);
			while (Mathf.Abs(Quaternion.Angle(go.transform.rotation, targetRot)) > .5f)
			{
				go.transform.rotation = Quaternion.Lerp(go.transform.rotation, targetRot, Time.deltaTime * ActorSpeed * 3);
				Debug.Log("Looking at exit");
				yield return null;
			}
			while ((go.transform.position - target).sqrMagnitude > .1f)
			{
				go.transform.position = Vector3.MoveTowards(go.transform.position, target, Time.deltaTime * ActorSpeed);
				Debug.Log("Exiting");
				yield return null;
			}

			yield return new WaitForSeconds(UnityEngine.Random.Range(ActorSpawnPeriod.x, ActorSpawnPeriod.y));
		}
	}
}