using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class LanguageUtility
{
	public const string CharacterSet = "เปิดکليلክፈትखोलनाдעפענעןکھلا" + // open
		"愛ପ୍ରେମمحبت사랑காதல்माया"; // Love

	public static string Generate(int length)
	{
		var sb = new StringBuilder();
		for(var i = 0; i < length; i++)
		{
			sb.Append(CharacterSet[UnityEngine.Random.Range(0, CharacterSet.Length - 1)]);
		}
		return sb.ToString();
	}

	public static string Translate(string str)
	{
		UnityEngine.Random.InitState(str.GetHashCode());
		var length = UnityEngine.Random.Range(3, str.Length);
		return Generate(length);
	}
}
