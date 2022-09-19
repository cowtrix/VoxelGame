using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul;

namespace Generation
{
	public class PipeGenerator : ExtendedMonoBehaviour, IGenerationCallback
	{
		public List<PipeConnector> Connectors => GetComponentsInChildren<PipeConnector>().ToList();
		public LineRenderer3D Line => GetComponent<LineRenderer3D>();
		public Guid LastGenerationID { get; set; }

		public void Generate(ObjectGenerator objectGenerator)
		{
			Invalidate();
		}

		[ContextMenu("Generate")]
		public void Invalidate()
		{
            if (!Line)
            {
				gameObject.AddComponent<LineRenderer3D>();
            }
			Line.Points = new List<Vector3>(Connectors.SelectMany(c => c.GetWorldspacePoints()).Select(p => transform.worldToLocalMatrix.MultiplyPoint3x4(p)));
			Line.RebakeMesh();
		}
	}
}