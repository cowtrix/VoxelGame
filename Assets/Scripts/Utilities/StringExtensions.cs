using UnityEngine;

namespace Common
{
    public static class StringExtensions
    {
        public static string SafeSubstring(this string str, int index, int length = -1)
		{
			if (string.IsNullOrEmpty(str))
			{
                return str;
			}
            if(length < 0)
			{
                length = str.Length - index;
			}
            if(length == 0)
			{
                return "";
			}
            length = Mathf.Min(length, str.Length - index);
            index = Mathf.Clamp(index, 0, str.Length - 1);
            return str.Substring(index, length);
		}

        public static string Truncate(this string value, int maxLength, string trailingString = null)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength) + trailingString;
        }

        public static string SplitCamelCase(this string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            s = s.Replace("_", " ");
            s = char.ToUpper(s[0]) + s.Substring(1);
            return System.Text.RegularExpressions.Regex.Replace(s, "(?<=[a-z])([A-Z])", " $1").Trim();
        }
    }
}