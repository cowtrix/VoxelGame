using System.Collections;
using UnityEngine;
using Voxul;
using Voxul.Utilities;

public abstract class UIStateUpdateEntry : ExtendedMonoBehaviour
{
	public UIStatusUpdate Parent;
	public PlayerState PlayerState { get; private set; }

	public float MoveSpeed = 1;
	public float ActiveTimeout = 5;
	protected float m_lastUpdate;
	public bool Active { get; protected set; }

	private Rect GetRect() => GetComponent<RectTransform>().rect;

	protected virtual void Awake()
	{
		PlayerState = CameraController.Instance.GetComponentInParent<PlayerState>();
	}

	private void Start()
	{
		StartCoroutine(Think());
	}

	private void Update()
	{
		var index = Parent.ActiveEntries.IndexOf(this);
		if (index >= 0)
		{
			if (Time.time > m_lastUpdate + ActiveTimeout)
			{
				// timed out, deactivate
				Active = false;
				return;
			}

			var rect = GetRect();
			Vector3 targetPos = new Vector2(0, (index * rect.height + 2));
			if(Vector3.Distance(transform.position, targetPos) > 0.01f)
			{
				transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, MoveSpeed * Time.deltaTime);
				return;
			}
		}
		else
		{
			var targetPos = new Vector3(0, 100);
			transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, MoveSpeed * Time.deltaTime);
		}
	}

	private IEnumerator Think()
	{
		while (true)
		{
			
			yield return null;
		}
	}
}
