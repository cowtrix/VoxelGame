using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxul;

namespace Portals
{
	public class Portal : ExtendedMonoBehaviour
	{
		public Camera Camera;
		public Portal Destination;

		public Vector3 PortalNormal = new Vector3(0, 0, 1);

		public RenderTexture Output;
		public Material PortalMaterial;
		public MeshRenderer Renderer;

		private Camera m_playerCamera;
		private static MaterialPropertyBlock m_propertyBlock;

		private void Start()
		{
			m_playerCamera = CameraController.Instance.GetComponent<Camera>();
			Output = new RenderTexture(Screen.width, Screen.height, 8);
			Camera.enabled = false;
			Camera.targetTexture = Output;
		}

		private void Update()
		{
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
			Camera.nearClipPlane = Vector3.Distance(Renderer.bounds.ClosestPoint(thisWorldPosition), thisWorldPosition);

			var portalBounds = Renderer.bounds;
			var screenRect = portalBounds.WorldBoundsToScreenRect(m_playerCamera);
			if (!screenRect.ScreenRectIsOnScreen())
			{
				Debug.Log($"Portal {this} is not onscreen, skipping");
				return;
			}

			// Update target
			//screenRect = screenRect.ClipToScreen();
			//Destination.Camera.rect = screenRect.ScreenRectToViewportRect();
			//Destination.Camera.fieldOfView = m_playerCamera.fieldOfView * (Screen.height / screenRect.height);
			Destination.Camera.Render();

			if (m_propertyBlock == null)
			{
				m_propertyBlock = new MaterialPropertyBlock();
			}
			m_propertyBlock.SetTexture("PortalTexture", Destination.Output);
			Renderer.SetPropertyBlock(m_propertyBlock);
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