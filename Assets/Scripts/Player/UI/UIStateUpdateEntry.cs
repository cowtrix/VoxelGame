using System.Collections;
using UnityEngine;
using Voxul;

public abstract class UIStateUpdateEntry : ExtendedMonoBehaviour
{
	public UIStatusUpdate Parent;
	public PlayerState PlayerState { get; private set; }

	public float MoveSpeed = 1;
	public float ActiveTimeout = 5;
	protected float m_lastUpdate;
	protected bool m_active;

	protected virtual void Awake()
	{
		PlayerState = CameraController.Instance.GetComponentInParent<PlayerState>();
		transform.SetParent(Parent.InactiveContainer);
		transform .localPosition = new Vector3(0, 0);
		StartCoroutine(Think());
	}

	private IEnumerator Think()
	{
		while (true)
		{
			if (m_active)
			{
				if (Time.time > m_lastUpdate + ActiveTimeout)
				{
					// timed out, deactivate
					m_active = false;
					continue;
				}
				var parentRectTransform = Parent.GetComponent<RectTransform>();
				if (transform.parent != parentRectTransform)
				{
					Vector3 targetPos;
					do
					{
						targetPos = new Vector3(0, parentRectTransform.position.y + parentRectTransform.sizeDelta.y)
							+ new Vector3(2, -4);
						yield return null;
						transform.position = Vector3.Lerp(transform.position, targetPos, MoveSpeed * Time.deltaTime);
					}
					while (Vector3.Distance(transform.position, targetPos) > 0.01f);
					transform.SetParent(Parent.transform);
				}
			}
			else
			{
				transform.SetParent(Parent.InactiveContainer);
				var targetPos = new Vector3(0, 0);
				transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, MoveSpeed * Time.deltaTime);
			}
			yield return null;
		}
	}
}
