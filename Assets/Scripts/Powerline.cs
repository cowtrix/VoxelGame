using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul.Utilities;

public class Powerline : MonoBehaviour
{
	public Vector2 Sag = new Vector2(-3, -4);
	public float Resolution = 1;
	public Powerline Next;
	public BezierConnectorLineRenderer[] ConnectorPoints;

	[ContextMenu("Randomize")]
	public void Randomize()
	{
		DisconnectAll();
		if (!Next)
		{
			foreach(var l in ConnectorPoints)
			{
				l.End.Transform = null;
			}
			UpdateSplines();
			return;
		}
		for (int i = 0; i < ConnectorPoints.Length; i++)
		{
			var r = ConnectorPoints[i];
			r.Resolution = Resolution;
			var nextEmpty = Next.ConnectorPoints
				.Where(nextPoint => !ConnectorPoints.Any(thisPoint => nextPoint.transform == thisPoint.End.Transform))
				.OrderBy(_ => Random.value)
				.FirstOrDefault();
			if (nextEmpty)
			{
				r.End.Transform = nextEmpty.transform;
			}
		}
		UpdateSplines();
	}

	public void UpdateSplines()
	{
		for (int i = 0; i < ConnectorPoints.Length; i++)
		{
			var r = ConnectorPoints[i];
			if (!r.End.Transform)
			{
				r.Invalidate();	
				continue;
			}
			var diff = (r.transform.worldToLocalMatrix.MultiplyVector(r.transform.position - r.End.Transform.position)
				.normalized * (Sag.y - Sag.x))
				.xz().x0z();
			var sag = r.transform.worldToLocalMatrix.MultiplyVector(Vector3.down * Random.Range(Sag.x, Sag.y));
			r.Start.Normal = diff - sag;
			r.End.Normal = diff - sag;
			r.Invalidate();
		}
	}

	public void DisconnectAll()
	{
		for (int i = 0; i < ConnectorPoints.Length; i++)
		{
			var r = ConnectorPoints[i];
			r.End.Transform = null;
		}
	}
}
