using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class PlayerEvent : UnityEvent<PlayerInteractionManager> { }

public class Interactable : MonoBehaviour
{
	public PlayerEvent OnFocusStart;
	public PlayerEvent OnFocus;
	public PlayerEvent OnFocusEnd;
	public PlayerEvent OnUsed;

	public Bounds Bounds => GetComponent<Collider>().bounds;
}
