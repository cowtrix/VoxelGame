using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Door : Interactable
{
	[Range(0, 1)]
	public float OpenAmount;
    public Vector3 OpenPosition, OpenRotation;
    public Vector3 ClosedPosition, ClosedRotation;

	public float Speed = 1;
	private float m_targetOpen;

	private void Reset()
	{
		OpenPosition = transform.localPosition;
		ClosedPosition = transform.localPosition;
	}

	private void OnValidate()
	{
		m_targetOpen = OpenAmount;
	}

	private void Start()
	{
		m_targetOpen = OpenAmount;
	}

	private void Update()
	{
		transform.localPosition = Vector3.Lerp(ClosedPosition, OpenPosition, OpenAmount);
		transform.localRotation = Quaternion.Lerp(Quaternion.Euler(ClosedRotation), Quaternion.Euler(OpenRotation), OpenAmount);

		OpenAmount = Mathf.MoveTowards(OpenAmount, m_targetOpen, Speed * Time.deltaTime);
	}

	public override void Use(Actor actor, string action)
	{
		if(m_targetOpen <= 0)
		{
			m_targetOpen = 1;
		}
		else
		{
			m_targetOpen = 0;
		}
		base.Use(actor, action);
	}
}
