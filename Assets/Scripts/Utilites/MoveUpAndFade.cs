using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUpAndFade : MonoBehaviour {

	CanvasGroup _canvasGroup;
	public float FadeSpeed = 1f;
	public float MoveSpeed = 1f;

	// Use this for initialization
	void Start () {
		_canvasGroup = gameObject.AddComponent<CanvasGroup>();
	}
	
	// Update is called once per frame
	void Update () {
		_canvasGroup.alpha -= FadeSpeed * Time.deltaTime;
		transform.localPosition += Vector3.up * MoveSpeed * Time.deltaTime;
		if(_canvasGroup.alpha <= 0)
		{
			Destroy(gameObject);
		}
	}
}
