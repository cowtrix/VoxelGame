using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Generation
{
	public class PipeConnector : MonoBehaviour
	{
		public List<Vector3> Points;
		public bool Reverse;

		public IEnumerable<Vector3> GetWorldspacePoints()
		{
			if (Points == null || !Points.Any())
			{
				yield return transform.position;
				yield break;
			}

			IEnumerable<Vector3> points = Points;
			if(Reverse)
			{
				points = points.Reverse();
			}
			foreach (var p in points)
			{
				yield return transform.localToWorldMatrix.MultiplyPoint3x4(p);
			}
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.matrix = transform.localToWorldMatrix;
			var s = Vector3.one * .1f;
			if (Points == null || !Points.Any())
			{
				Gizmos.DrawWireCube(Vector3.zero, s);
				return;
			}

			foreach (var p in Points)
			{
				Gizmos.DrawWireCube(p, s);
			}
		}
	}
}