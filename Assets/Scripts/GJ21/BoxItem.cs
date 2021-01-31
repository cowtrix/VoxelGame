using System;
using System.Collections.Generic;
using UnityEngine;

public class BoxItem : InteractableItem 
{
	public AudioClip OpenClip, CloseClip;
	public Transform CameraTransform;
	public bool Open { get; private set; }
	public Vector3 OpenVector = new Vector3(0, 0, 1);
	public Vector3 InsertPosition = new Vector3(0, .5f, 0);
	public float MoveSpeed = 1;

	public HashSet<GameItem> StoredItems;

	private Vector3 m_startPos;
	Rigidbody Rigidbody => GetComponent<Rigidbody>();
	AudioSource Audio => GetComponent<AudioSource>();

	private void Awake()
	{
		m_startPos = Rigidbody.position;
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
}
