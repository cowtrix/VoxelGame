using System;

public class StateMinMaxAttribute : Attribute
{
	public readonly float Min;
	public readonly float Max;
	public StateMinMaxAttribute(float min, float max)
	{
		Min = min;
		Max = max;
	}
}
