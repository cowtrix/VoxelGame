using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class PlayerEvent : UnityEvent<PlayerInteractionManager> { }

[Serializable]
public class SpriteEvent : UnityEvent<Sprite> { }

public class Interactable : MonoBehaviour
{
	public float MinimumDistance = 0;
	public float MaximumDistance = 10;
	public PlayerEvent OnFocusStart;
	public PlayerEvent OnFocus;
	public PlayerEvent OnFocusEnd;
	public PlayerEvent OnUsed;
	public PlayerEvent OnEnterInteractionZone;
	public PlayerEvent OnExitInteractionZone;

	public Func<Sprite> Icon;

	public Bounds Bounds => GetComponent<Collider>().bounds;
}
