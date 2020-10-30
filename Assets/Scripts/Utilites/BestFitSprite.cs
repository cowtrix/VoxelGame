using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BestFitSprite : MonoBehaviour 
{
	SpriteRenderer _spriteRenderer;

	void Update () {
		if(!_spriteRenderer)
		{
			_spriteRenderer = GetComponent<SpriteRenderer>();
		}
		if(!_spriteRenderer.sprite)
		{
			_spriteRenderer.enabled = false;
			return;
		}
		_spriteRenderer.enabled = true;
		var spriteBounds = _spriteRenderer.sprite.bounds;
		var scale = transform.localScale;
		var cam = Camera.main;
		var orthoSize = cam.orthographicSize;

		transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, transform.position.z);
		transform.localScale = Vector3.one * (1 / spriteBounds.size.y) * orthoSize * 2f;
	}
}
