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
    private PlayerInput m_input;
    private InputAction m_sizeUp, m_sizeDown;

	private void Awake()
	{
		m_input = GetComponent<PlayerInput>();
		m_sizeUp = m_input.actions.Single(a => a.name == "SizeUp");
		m_sizeDown = m_input.actions.Single(a => a.name == "SizeDown");
	}

	private void Update()
	{
		if (m_sizeUp.triggered)
		{
			Debug.Log("Sizeup");
			var colls = Physics.OverlapBox(transform.position,
				transform.localToWorldMatrix.MultiplyVector(PlayerBounds.extents),
				transform.rotation, CollisionMask);
			if (colls.Any())
			{
				return;
			}
			transform.localScale *= 3;
		}
		else if(m_sizeDown.triggered)
		{
			Debug.Log("Sizedown");
			transform.localScale /= 3;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(PlayerBounds.center, PlayerBounds.extents);
	}
}
