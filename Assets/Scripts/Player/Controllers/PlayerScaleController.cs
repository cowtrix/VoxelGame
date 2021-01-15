using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class PlayerScaleController : MonoBehaviour
{
	public LayerMask CollisionMask;
	public Bounds PlayerBounds;
	public float TransitionSpeed = 1;

	private float m_targetScale = 1;
	private float m_currentScale = 1;

	private void Update()
	{
		var effector = UniverseEffector.Instances
			.Where(b => new Bounds(b.transform.position, b.Size).Contains(transform.position))
			.OrderBy(b => b.Scale)
			.FirstOrDefault();
		if(effector)
		{
			Debug.Log($"Set scale to {effector.Scale}");
			m_targetScale = effector.Scale;
		}
		else
		{
			m_targetScale = 1;
		}

		var step = Mathf.MoveTowards(m_currentScale, m_targetScale, TransitionSpeed * Time.deltaTime);
		var colls = Physics.OverlapBox(transform.position,
				transform.localToWorldMatrix.MultiplyVector(PlayerBounds.extents * step),
				transform.rotation, CollisionMask);
		if (!colls.Any())
		{
			m_currentScale = step;
		}
		transform.localScale = Vector3.one * m_currentScale;
	}

	private void OnDrawGizmos()
	{
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(PlayerBounds.center, PlayerBounds.extents);
	}
}
