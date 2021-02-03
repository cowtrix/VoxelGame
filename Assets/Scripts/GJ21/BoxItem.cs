using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoxItem : InteractableItem 
{
	public AudioClip OpenClip, CloseClip, BadClip;
	public Transform CameraTransform;
	public bool Open { get; private set; }
	public Vector3 OpenVector = new Vector3(0, 0, 1);
	public Vector3 InsertPosition = new Vector3(0, .5f, 0);
	public float MoveSpeed = 1;

	public List<GameItem> StoredItems => Physics.OverlapBox(transform.position, Vector3.one * .45f, transform.rotation)
		.Select(c => c.GetComponent<GameItem>())
		.Where(c => c)
		.ToList();

	private Vector3 m_startPos;
	Rigidbody Rigidbody => GetComponent<Rigidbody>();
	AudioSource Audio => GetComponent<AudioSource>();

	private void Awake()
	{
		m_startPos = Rigidbody.position;
		StartCoroutine(ProcessConflicts());
	}

	public override string Verb => !Open ? "Open" : InputManager.Instance.HeldItem ? "Place Item" : "Close";

	private void Update()
	{
		var targetPos = m_startPos;
		if(Open)
		{
			targetPos += OpenVector;
		}
		Rigidbody.position =
				Vector3.MoveTowards(Rigidbody.position, targetPos, MoveSpeed * Time.deltaTime);
	}

	public void DelayClose(float v)
	{
		if(!Open)
		{
			return;
		}
		Invoke(nameof(Close), v);
	}

	private void Close()
	{
		SetOpen(false);
	}

	public void SetOpen(bool open)
	{
		if(Open == open)
		{
			return;
		}
		Open = open;
		if(Open)
		{
			Audio.PlayOneShot(OpenClip);
		}
		else
		{
			Audio.PlayDelayed(.5f);
		}
	}

	public IEnumerator ProcessConflicts()
	{
		while(true)
		{
			yield return new WaitForSeconds(1 + UnityEngine.Random.value);

			foreach(var it in StoredItems)
			{
				Debug.Log("THought about " + it);
				bool explode = false;
				bool implode = false;
				foreach(var t in it.Traits)
				{
					switch (t)
					{
						case eItemTrait.Electric:
							implode |= StoredItems.Any(i => i.Traits.Contains(eItemTrait.Wet));
							break;
						case eItemTrait.Flammable:
							explode |= StoredItems.Any(i => i.Traits.Contains(eItemTrait.Electric));
							implode = explode;
							break;
						case eItemTrait.Wet:
							implode |= StoredItems.Any(i => i.Traits.Contains(eItemTrait.Flammable));
							break;
					}
				}
				if(explode)
				{
					implode = true;
				}
				if(implode)
				{
					Debug.Log($"impledod {it}");
					var pos = it.transform.position;
					var rot = it.transform.rotation;
					Destroy(it.gameObject);
					var go = Instantiate(LostFoundGameManager.Instance.CoalLump.gameObject);
					var rb = go.GetComponent<Rigidbody>();
					rb.position = pos;
					rb.rotation = rot;
					Audio.PlayOneShot(BadClip);
					break;
				}
			}
		}
	}
}
