using Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul;
using Voxul.Utilities;

namespace Portals
{
	public class Portal : ExtendedMonoBehaviour
	{
		public Camera Camera;
		public Portal Destination;
		public float PortalDistance = 10;
		public AnimationCurve FadeCurve;
		public Vector3 PortalNormal = new Vector3(0, 0, 1);

		public RenderTexture Output;
		public Material PortalMaterial;
		public Renderer[] Renderers { get; private set; }
		public Bounds Bounds { get; private set; }

		private Camera m_playerCamera;
		private static MaterialPropertyBlock m_propertyBlock;

		private void Start()
		{
			Renderers = GetComponentsInChildren<Renderer>();
			m_playerCamera = CameraController.Instance.GetComponent<Camera>();
			Output = new RenderTexture(Screen.width, Screen.height, 8);
			Camera.enabled = false;
			Camera.targetTexture = Output;
			RecalculatBounds();
		}

		public void RecalculatBounds()
		{
			Bounds = Renderers.Select(r => r.bounds).GetEncompassingBounds();
		}

		private void Update()
		{
			if (!Destination)
			{
				return;
			}
			var playerPosition = m_playerCamera.transform.position;
			var playerForward = m_playerCamera.transform.forward;

			Debug.DrawLine(playerPosition, playerPosition + playerForward, Color.green);

			var destLocalNormal = Destination.PortalNormal;
			var destLocalForward = Destination.transform.worldToLocalMatrix.MultiplyVector(playerForward);
			var destLocalPosition = Destination.transform.worldToLocalMatrix.MultiplyPoint3x4(playerPosition);

			var flipRot = Matrix4x4.Rotate(Quaternion.LookRotation(-destLocalNormal));

			var thisWorldForward = transform.localToWorldMatrix.MultiplyVector(flipRot.MultiplyVector(destLocalForward));
			var thisWorldPosition = transform.localToWorldMatrix.MultiplyPoint3x4(flipRot.MultiplyPoint3x4(destLocalPosition));

			Camera.transform.position = thisWorldPosition;
			Camera.transform.forward = thisWorldForward;
			Camera.nearClipPlane = Mathf.Max(.01f, Vector3.Distance(Bounds.ClosestPoint(thisWorldPosition), thisWorldPosition));

			if (Vector3.Distance(m_playerCamera.transform.position, transform.position) > PortalDistance ||
				!m_playerCamera.BoundsWithinFrustrum(Bounds))
			{
				return;
			}

			var screenRect = Bounds.WorldBoundsToScreenRect(m_playerCamera);
			/*if (!screenRect.ScreenRectIsOnScreen())
			{
				//Debug.Log($"Portal {this} is not onscreen, skipping");
				return;
			}*/

			// Update target
			screenRect = screenRect.ClipToScreen();
			if (screenRect.width <= 0 || screenRect.height <= 0)
			{
				return;
			}
			CameraScissorRectUtility.SetScissorRect(Destination.Camera, screenRect.ScreenRectToViewportRect());
			//Destination.Camera.fieldOfView = m_playerCamera.fieldOfView * (Screen.height / screenRect.height);
			Destination.Camera.Render();
		}

		private void LateUpdate()
		{
			if (m_propertyBlock == null)
			{
				m_propertyBlock = new MaterialPropertyBlock();
			}

			if (!Destination)
			{
				m_propertyBlock.SetFloat("DistanceFade", 1);
				return;
			}

			m_propertyBlock.SetTexture("PortalTexture", Destination.Output);
			var distanceFloat = FadeCurve.Evaluate(1 - Mathf.Clamp01(Vector3.Distance(m_playerCamera.transform.position, transform.position) / PortalDistance));
			m_propertyBlock.SetFloat("DistanceFade", distanceFloat);
			foreach (var r in Renderers)
			{
				r.SetPropertyBlock(m_propertyBlock);
			}
		}

		private void OnDestroy()
		{
			Destroy(Output);
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.color = Color.magenta;
			Gizmos.DrawLine(Vector3.zero, PortalNormal);
		}
	}
}