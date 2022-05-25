using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class LanguageUtility
{
    public static string CharacterSet;

    static LanguageUtility()
    {
        CharacterSet = Resources.Load<TextAsset>("AlienCharacters").text;
    }

    public static string Generate(int length)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < length; i++)
        {
            sb.Append(CharacterSet[UnityEngine.Random.Range(0, CharacterSet.Length - 1)]);
        }
        return sb.ToString();
    }

    public static string Translate(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }
        UnityEngine.Random.InitState(str.GetHashCode());
        var length = UnityEngine.Random.Range(3, str.Length);
        return Generate(length);
    }

    public static string GetSpriteKeyForChar(char c)
    {
        if(c == ' ')
        {
            return "Space";
        }
        return c.ToString();
    }

    public static string GetStringForTextMesh(string original)
    {
        var sb = new StringBuilder();
        var isAlien = false;
        foreach (var c in original)
        {
            if (c == '{')
            {
                isAlien = true;
                continue;
            }
            else if (c == '}')
            {
                isAlien = false;
                continue;
            }
            if (isAlien && c != '\n')
            {
                sb.Append($"<sprite name=\"{GetSpriteKeyForChar(c)}\" tint=1>");
            }
            else
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }
}
