using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class UIExtensions {
	static Vector3[] _worldRectCache = new Vector3[4];
	public static Rect GetWorldRect(this RectTransform rect)
	{
		rect.GetWorldCorners(_worldRectCache);
		return new Rect(_worldRectCache[0].x, _worldRectCache[0].y, _worldRectCache[2].x - _worldRectCache[0].x, _worldRectCache[2].y - _worldRectCache[0].y);
	}

	public static Bounds GetBounds(this Canvas canvas)
    {
		var b = new Bounds(canvas.transform.position, Vector3.zero);
		var rt = canvas.GetComponent<RectTransform>();
		rt.GetWorldCorners(_worldRectCache);
		foreach(var p in _worldRectCache)
        {
			b.Encapsulate(p);
        }
		return b;
    }
}
