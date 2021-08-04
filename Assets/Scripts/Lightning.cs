using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxul;

[ExecuteAlways]
[RequireComponent(typeof(LineRenderer))]
public class Lightning : ExtendedMonoBehaviour
{
    public LineRenderer Renderer => GetComponent<LineRenderer>();

    public Vector3 Offset = Vector3.down;
    public int Resolution = 10;
	public float Jiggle = .1f;
    public Vector3 Frequency = new Vector3(1, 0, 1);

	private Vector3[] m_points;

	private void Update()
	{
		Renderer.positionCount = Resolution; 
		if(m_points == null || m_points.Length != Resolution)
		{
			m_points = new Vector3[Resolution];
		}
		var origin = Vector3.zero;
		for (int i = 0; i < m_points.Length; i++)
		{
			var t = (i + Random.Range(-Jiggle, Jiggle)) / ((float)Resolution);
			var shift = new Vector3(Frequency.x * Random.Range(-1f, 1f), Frequency.y * Random.Range(-1f, 1f), Frequency.z * Random.Range(-1f, 1f));
			m_points[i] = Offset * t + shift;
		}
		Renderer.SetPositions(m_points);
	}
}
