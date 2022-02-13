using Common;
using Voxul;

namespace Generation
{
	public class PowerPoint : TrackedObject<PowerPoint>
	{
		public BezierConnectorLineRenderer Line => GetComponent<BezierConnectorLineRenderer>();
	}
}