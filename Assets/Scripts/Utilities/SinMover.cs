using UnityEngine;

namespace Common
{
	public class SinMover : MonoBehaviour
	{
		public float Speed = 1;
		public bool RotationEnabled = true, PositionEnabled = true, RandomYSpin;
		public Vector3 StartPosition, StartRotation;
		public Vector3 EndPosition, EndRotation;

		private Rigidbody m_rigidBody;

		private void Reset()
		{
			StartPosition = transform.localPosition;
			StartRotation = transform.localRotation.eulerAngles;
			EndPosition = transform.localPosition;
			EndRotation = transform.localRotation.eulerAngles;
		}

		private void Start()
		{
			m_rigidBody = GetComponent<Rigidbody>();
			if (RandomYSpin)
			{
				var randY = Random.Range(0, 360);
				StartRotation.y = randY;
				EndRotation.y = randY;
			}
		}

		private void Update()
		{
			var t = (Mathf.Sin(Time.time * Speed) + 1) / 2;
			if (PositionEnabled)
			{
				var targetPos = Vector3.Lerp(EndPosition, StartPosition, t);
				if (m_rigidBody)
				{
					m_rigidBody.MovePosition(transform.localToWorldMatrix.MultiplyPoint3x4(targetPos));
				}
				else
				{
					transform.localPosition = targetPos;
				}

			}
			if (RotationEnabled)
			{
				var targetRot = Quaternion.Euler(Vector3.Lerp(EndRotation, StartRotation, t));
				if (m_rigidBody)
				{
					m_rigidBody.MoveRotation(transform.rotation * targetRot);
				}
				else
				{
					transform.localRotation = targetRot;
				}

			}
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.DrawLine(StartPosition, EndPosition);
		}
	}
}