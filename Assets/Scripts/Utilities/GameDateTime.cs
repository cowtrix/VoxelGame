using System;
using UnityEngine;

[Serializable]
public struct GameDateTime
{
	public const int HoursInDay = 27;

	public int DayNumber;

	[HideInInspector]
	public float NormalizedTime;

	private float FloatRepresentation => DayNumber + NormalizedTime;

	[Range(0, 27)]
	public int Hour;
	[Range(0, 60)]
	public int Minute;
	[Range(0, 60)]
	public int Second;
	
	public string GetShortTimeString() => $"{Hour:00}:{Minute:00}";
	public string GetLongTimeString() => $"{Hour:00}:{Minute:00}:{Second:00}";
	public string GetDateString() => $"Day {DayNumber}";

	public override string ToString() => GetLongTimeString();

	public override bool Equals(object obj)
	{
		return obj is GameDateTime time &&
			   DayNumber == time.DayNumber &&
			   Hour == time.Hour &&
			   Minute == time.Minute &&
			   Second == time.Second;
	}

	public override int GetHashCode()
	{
		int hashCode = -880199227;
		hashCode = hashCode * -1521134295 + DayNumber.GetHashCode();
		hashCode = hashCode * -1521134295 + Hour.GetHashCode();
		hashCode = hashCode * -1521134295 + Minute.GetHashCode();
		hashCode = hashCode * -1521134295 + Second.GetHashCode();
		return hashCode;
	}

	public GameDateTime(int dayNumber, float normalisedTime)
	{
		DayNumber = dayNumber;
		NormalizedTime = normalisedTime;

		var time = normalisedTime * HoursInDay;
		Hour = Mathf.FloorToInt(time);
		var mF = time - Hour;
		var sF = time - Hour - Mathf.FloorToInt(mF);
		Minute = Mathf.FloorToInt(mF * 60);
		Second = Mathf.FloorToInt(sF * 60 * 60);
	}

	public static bool operator ==(GameDateTime left, GameDateTime right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(GameDateTime left, GameDateTime right)
	{
		return !(left == right);
	}

	public static bool operator <(GameDateTime left, GameDateTime right)
	{
		return left.FloatRepresentation < right.FloatRepresentation;
	}

	public static bool operator >(GameDateTime left, GameDateTime right)
	{
		return left.FloatRepresentation > right.FloatRepresentation;
	}
}
