using System;

public class StateMinAttribute : Attribute
{
	public readonly float Min;
	public StateMinAttribute(float min)
	{
		Min = min;
	}
}
