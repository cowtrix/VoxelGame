using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul;
using Voxul.Utilities;

namespace Generation
{
	public class Powerline : ExtendedMonoBehaviour, IGenerationCallback
	{
		public Vector2 Sag = new Vector2(-3, -4);
		public float Resolution = 1;
		public float ConnectionDistance = 50;
		public Powerline Next;
		public BezierConnectorLineRenderer[] ConnectorPoints;

		public System.Guid LastGenerationID { get; set; }

		[ContextMenu("Generate")]
		public void Invalidate()
		{
			Generate(null);
		}

		public void Generate(ObjectGenerator objectGenerator)
		{
			DisconnectAll();

			foreach(var neighbour in PowerPoint.Instances.Where(p => Vector3.Distance(p.transform.position, transform.position) < ConnectionDistance))
			{
				Debug.DrawLine(neighbour.transform.position, transform.position, Color.green, 10);
				var nLine = neighbour.Line;
				nLine.EndTransform.Transform = ConnectorPoints.Random().StartTransform.Transform;
				var diff = (nLine.transform.worldToLocalMatrix.MultiplyVector(nLine.transform.position - nLine.EndTransform.Transform.position)
					.normalized * (neighbour.Sag.y - neighbour.Sag.x))
					.xz().x0z();
				var sag = nLine.transform.worldToLocalMatrix.MultiplyVector(Vector3.down * Random.Range(neighbour.Sag.x, neighbour.Sag.y));
				nLine.StartTransform.Normal = diff - sag;
				nLine.EndTransform.Normal = diff - sag;
				nLine.Resolution = Resolution;
				nLine.Invalidate();
			}

			if (!Next)
			{
				foreach (var l in ConnectorPoints)
				{
					l.EndTransform.Transform = null;
				}
				UpdateSplines();
				return;
			}
			for (int i = 0; i < ConnectorPoints.Length; i++)
			{
				var r = ConnectorPoints[i];
				r.Resolution = Resolution;
				var nextEmpty = Next.ConnectorPoints
					.Where(nextPoint => !ConnectorPoints.Any(thisPoint => nextPoint.transform == thisPoint.EndTransform.Transform))
					.OrderBy(_ => Random.value)
					.FirstOrDefault();
				if (nextEmpty)
				{
					r.EndTransform.Transform = nextEmpty.transform;
				}
			}
			UpdateSplines();
		}

		public void UpdateSplines()
		{
			for (int i = 0; i < ConnectorPoints.Length; i++)
			{
				var r = ConnectorPoints[i];
				if (!r.EndTransform.Transform)
				{
					r.Invalidate();
					continue;
				}
				var diff = (r.transform.worldToLocalMatrix.MultiplyVector(r.transform.position - r.EndTransform.Transform.position)
					.normalized * (Sag.y - Sag.x))
					.xz().x0z();
				var sag = r.transform.worldToLocalMatrix.MultiplyVector(Vector3.down * Random.Range(Sag.x, Sag.y));
				r.StartTransform.Normal = diff - sag;
				r.EndTransform.Normal = diff - sag;
				r.Invalidate();
			}
		}

		public void DisconnectAll()
		{
			for (int i = 0; i < ConnectorPoints.Length; i++)
			{
				var r = ConnectorPoints[i];
				r.EndTransform.Transform = null;
			}
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.DrawWireSphere(Vector3.zero, ConnectionDistance);
		}
	}
}