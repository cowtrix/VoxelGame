using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Phone
{
	public class PhoneAppCamera : PhoneApp
	{
		public Camera Camera;
		public RawImage Image;
		public Vector2Int Resolution = new Vector2Int(200, 300);

		private void Update()
		{
			var rt = RenderTexture.GetTemporary(Resolution.x, Resolution.y);
			Camera.targetTexture = rt;
			Camera.Render();
			Image.texture = rt;
			Camera.targetTexture = null;
			RenderTexture.ReleaseTemporary(rt);
		}

		public void TakePhoto()
		{

		}
	}
}