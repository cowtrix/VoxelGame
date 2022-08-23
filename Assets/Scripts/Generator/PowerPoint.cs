using Common;
using UnityEngine;
using Voxul;

namespace Generation
{
	public class PowerPoint : TrackedObject<PowerPoint>
	{
		public BezierConnectorLineRenderer Line => GetComponent<BezierConnectorLineRenderer>();
		public Vector2 Sag;
	}
}