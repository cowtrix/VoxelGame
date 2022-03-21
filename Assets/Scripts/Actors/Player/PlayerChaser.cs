using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SmoothPositionVector3
{
	private Queue<Vector3> m_positions;
	public int Capacity { get; set; }
	public int Count => m_positions.Count;
	public Vector3 SmoothPosition => m_positions.Aggregate((x, y) => x + y) / m_positions.Count;

	public SmoothPositionVector3(int count, Vector3 position)
	{
		Capacity = count;
		m_positions = new Queue<Vector3>(new[] { position });
	}

	public void Push(Vector3 pos)
	{
		m_positions.Enqueue(pos);
		while (m_positions.Count > Capacity)
		{
			m_positions.Dequeue();
		}
	}
}

public class PlayerChaser : MonoBehaviour
{
	public float LerpSpeed = .1f;
	public Transform Transform;
	public int SmoothCount = 10;
	SmoothPositionVector3 m_smoothPos;

	private void Awake()
	{
		m_smoothPos = new SmoothPositionVector3(SmoothCount, transform.position);
	}

	private void Update()
	{
		m_smoothPos.Push(Transform.position);
		var dt = LerpSpeed * Time.deltaTime;
		transform.position = Vector3.Lerp(transform.position, m_smoothPos.SmoothPosition, dt);
		transform.localScale = Vector3.Lerp(transform.localScale, Transform.localScale, dt);
		transform.rotation = Quaternion.Lerp(transform.rotation, Transform.rotation, dt);
	}
}
