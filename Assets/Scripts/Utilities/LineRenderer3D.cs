using System;
using System.Collections.Generic;
using UnityEngine;
using Voxul;
using Voxul.Utilities;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class LineRenderer3D : ExtendedMonoBehaviour
{
	public int CircularResolution = 5;
	public Material Material;
	public bool Loop;
	public float Radius = .2f;
	public AnimationCurve RadiusOverLifetime = AnimationCurve.Linear(0, 1, 1, 1);
	public List<Vector3> Points = new List<Vector3>();
	public bool GenerateCollider;

	private Mesh Mesh
	{
		get
		{
			return Filter.sharedMesh;
		}
		set
		{
			Filter.sharedMesh = value;
		}
	}
	private List<int> _triangleBuffer = new List<int>();
	private List<Vector4> _uvBuffer = new List<Vector4>();
	private List<Vector3> _vertexBuffer = new List<Vector3>();

	protected MeshRenderer Renderer => GetComponent<MeshRenderer>();
	protected MeshFilter Filter => GetComponent<MeshFilter>();
	protected MeshCollider Collider => gameObject.GetOrAddComponent<MeshCollider>();
	protected void GetNormalTangent(int i, out Vector3 normal, out Vector3 tangent)
	{
		if (Points.Count < 2)
		{
			normal = Vector3.zero;
			tangent = Vector3.zero;
			return;
		}

		var position = Points[i];
		Vector3 lastPoint, nextPoint;
		if (i == 0)
		{
			lastPoint = Loop ? Points[Points.Count - 1] : position;
			nextPoint = Points[1];
		}
		else if (i == Points.Count - 1)
		{
			lastPoint = Points[Points.Count - 2];
			nextPoint = Loop ? Points[0] : position;
		}
		else
		{
			lastPoint = Points[i - 1];
			nextPoint = Points[i + 1];
		}

		normal = ((lastPoint - position) + (position - nextPoint)) / 2;
		tangent = Vector3.Max(Vector3.Cross(normal, transform.up), Vector3.Max(Vector3.Cross(normal, transform.right), Vector3.Cross(normal, transform.forward))).normalized;

		normal.Normalize();
		tangent.Normalize();
	}

	[ContextMenu("Rebake")]
	public void RebakeMesh()
	{
		if (Mesh == null)
		{
			Mesh = new Mesh();
		}
		Renderer.sharedMaterial = Material;
		_vertexBuffer.Clear();
		_triangleBuffer.Clear();
		_uvBuffer.Clear();
		Mesh.Clear();

		if (Points.Count < 2)
		{
			return;
		}

		if (Loop)
		{
			Points.Add(Points[0]);
		}

		for (var i = Points.Count - 1; i >= 0; i--)
		{
			var position = transform.localToWorldMatrix.MultiplyPoint3x4(Points[i]);
			var maxPoints = (float)GetMaxPoints();
			var lifetime = ((Points.Count - i - 1) / maxPoints);

			Vector3 normal, tangent;
			GetNormalTangent(i, out normal, out tangent);
			Debug.DrawLine(position, position + normal, Color.red, 5);
			Debug.DrawLine(position, position + tangent, Color.green, 5);
			var anglestep = 360 / CircularResolution;
			for (var step = 0; step < CircularResolution; step++)
			{
				var angle = step * anglestep;
				var radius = Radius * RadiusOverLifetime.Evaluate(lifetime);
				var circlePosition = position + Quaternion.AngleAxis(angle, normal)
									 * tangent * radius;
				circlePosition = transform.InverseTransformPoint(circlePosition);

				// Add vertex
				_vertexBuffer.Add(circlePosition);
				_uvBuffer.Add(new Vector4((step / (float)(CircularResolution - 1)), lifetime));
				if (i == Points.Count - 1)
				{
					continue;
				}

				// Add tris
				var p1 = _vertexBuffer.Count - 1;
				var p2 = p1 - CircularResolution;
				var p3 = p1 + 1;
				var p4 = p2 + 1;
				if (step == CircularResolution - 1)
				{
					p3 -= CircularResolution;
					p4 -= CircularResolution;
				}
				_triangleBuffer.Add(p1);
				_triangleBuffer.Add(p2);
				_triangleBuffer.Add(p3);

				_triangleBuffer.Add(p3);
				_triangleBuffer.Add(p2);
				_triangleBuffer.Add(p4);
			}
		}

		if (Loop)
		{
			Points.RemoveAt(Points.Count - 1);
		}

		Mesh.SetVertices(_vertexBuffer);
		Mesh.SetTriangles(_triangleBuffer, 0);
		Mesh.SetUVs(0, _uvBuffer);
		Mesh.RecalculateNormals();

		if (GenerateCollider)
		{
			Collider.sharedMesh = Mesh;
		}
	}

	protected virtual int GetMaxPoints()
	{
		return Points.Count;
	}

	public void Simplify(float threshold)
	{
		for (int i = Points.Count - 2; i > 0; i--)
		{
			var vector3 = Points[i];
			var next = Points[i - 1];
			if (Vector3.Distance(vector3, next) < threshold)
			{
				Points.RemoveAt(i);
			}
		}
	}
}