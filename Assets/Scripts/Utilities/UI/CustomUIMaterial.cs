using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class CustomUIMaterial : UIBehaviour
{
    public Canvas Canvas;
    public Material Material;

	protected override void OnCanvasHierarchyChanged()
	{
		base.OnCanvasHierarchyChanged();
		CrawlHierarchy(transform);
	}

	[ContextMenu("Invalidate")]
	public void Invalidate() => CrawlHierarchy(transform);

	void CrawlHierarchy(Transform t)
	{
		var renderer = t.GetComponent<Graphic>();
		if (renderer)
		{
			renderer.material = Material;
		}
		foreach(Transform child in t)
		{
			CrawlHierarchy(child);
		}
	}
}
