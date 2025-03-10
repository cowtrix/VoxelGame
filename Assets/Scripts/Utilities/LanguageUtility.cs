﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class LanguageUtility
{
    private static Dictionary<string, string> m_quickCharLookup = new Dictionary<string, string>();
    public const string CharacterSet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    static char[] m_unsupportedCharacters = new[]
        {
            '?', '<', '>', '\''
        };
    public static string Generate(int length, int? seed = null)
    {
        if (seed.HasValue)
        {
            UnityEngine.Random.InitState(seed.Value);
        }
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
        return GetStringForTextMesh(Generate(length));
    }

    public static string GetSpriteKeyForChar(char c)
    {
        if (m_unsupportedCharacters.Contains(c))
        {
            return "";
        }
        var key = c.ToString();
        if (c == ' ')
        {
            key = "Space";
        }
        if(!m_quickCharLookup.TryGetValue(key, out var result))
        {
            result = $"<sprite name=\"{key}\" tint=1>";
            m_quickCharLookup[key] = result;
        }
        return result;
    }

    public static string GetStringForTextMesh(string original, bool mixed = false)
    {
        char[] reservedChars = new[]
        {
            '\n', '\r'
        };
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
            if ((!mixed || isAlien) && !reservedChars.Contains(c))
            {
                sb.Append(GetSpriteKeyForChar(c));
            }
            else
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }

    public static string GetGarbageText(int length)
    {
        const string charSet = CharacterSet;
        var sb = new StringBuilder();
        for (var i = 0; i < length; i++)
        {
            var roll = UnityEngine.Random.value;
            if (roll < .1f)
            {
                sb.Append(' ');
            }
            else if (roll < .125f)
            {
                sb.Append('\n');
            }
            else if (roll < .15f)
            {
                sb.Append('.');
            }
            else
            {
                sb.Append(charSet.Random());
            }
        }
        return sb.ToString();
    }
}
