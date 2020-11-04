using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class PlayerEvent : UnityEvent<PlayerController> { }

public class Interactable : MonoBehaviour
{
	public PlayerEvent OnUsed;

	public Bounds Bounds => GetComponent<Collider>().bounds;
}
