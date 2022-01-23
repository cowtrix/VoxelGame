using Common;
using Items;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul;

public class ClawMachineClaw : ExtendedMonoBehaviour
{
	public Bounds DetectionPoint;
	public LayerMask CollisionMask;
	public float ClawSpring = 1;
	public float ClawBreakForce = 1;
	public float ClawDampening = 1;
	private SpringJoint m_joint;
	public float TargetOpenAmount { get; set; }
	public bool CheckCollision { get; set; }
	public Lightning Lightning { get; private set; }

	private void Start()
	{
		Lightning = GetComponentInChildren<Lightning>();
	}

	private void Update()
	{
		transform.localPosition = Vector3.zero;
		var overlap = Physics.OverlapBox(
		   transform.localToWorldMatrix.MultiplyPoint3x4(DetectionPoint.center),
		   transform.localToWorldMatrix.MultiplyVector(DetectionPoint.extents),
		   transform.rotation, CollisionMask);
		m_joint = GetComponent<SpringJoint>();
		if (CheckCollision)
		{
			var firstItem = overlap.FirstOrDefault(o => o.GetComponent<Item>() || o.transform.parent?.GetComponent<Item>())?
				.GetComponent<Rigidbody>();
			if (firstItem)
			{
				Lightning.gameObject.SetActive(true);
				Lightning.Offset = Lightning.transform.worldToLocalMatrix.MultiplyPoint3x4(firstItem.position);
				if (!m_joint)
				{
					m_joint = gameObject.AddComponent<SpringJoint>();
					m_joint.autoConfigureConnectedAnchor = false;
					m_joint.connectedBody = firstItem;
					m_joint.anchor = DetectionPoint.center;
					m_joint.connectedAnchor = Vector3.zero; 
					m_joint.maxDistance = float.Epsilon;
					m_joint.spring = ClawSpring;
					m_joint.breakForce = ClawBreakForce;
					m_joint.damper = ClawDampening;
					Debug.Log($"Claw colliding with {overlap.First()}", overlap.First());
				}				
				return;
			}
		}
		else if(m_joint)
		{
			Destroy(m_joint);
		}
		Lightning.gameObject.SetActive(false);
		transform.localRotation = Quaternion.Euler(new Vector3(0, transform.localRotation.eulerAngles.y, Mathf.Lerp(-10, -35, TargetOpenAmount)));
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawWireCube(DetectionPoint.center, DetectionPoint.size);
	}
}
