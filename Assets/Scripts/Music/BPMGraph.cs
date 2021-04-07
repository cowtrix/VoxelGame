using System;

public class BPMGraph : DebugGraph
{
	public enum Type
	{
		BPM_Frac,
		BPM_4Bar,
		BPM_Sawtooth,
		Time_Until_Next_Bar,
	}
	public Type GraphType;

	protected override float Value
	{
		get
		{
			switch (GraphType)
			{
				case Type.BPM_Frac:
					return (float)BeatManager.Instance.BPMFrac;
				case Type.BPM_4Bar:
					return (float)BeatManager.Instance.BPMFourBar;
				case Type.BPM_Sawtooth:
					return (float)BeatManager.Instance.BPMFrac;
				case Type.Time_Until_Next_Bar:
					return (float)(BeatManager.Instance.SecondsUntilNextBar / BeatManager.Instance.OneBarTime);
			}
			throw new NotSupportedException(GraphType.ToString());
	}
}

protected override string LabelText => GraphType.ToString();


}
