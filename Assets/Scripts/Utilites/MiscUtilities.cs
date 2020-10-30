using MadMaps.Common.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MadMaps.Common
{
    public static class  MiscUtilities
    {
        public static float FloorToUshort(float f)
        {
            f = Mathf.Clamp01(f);
            ushort u = (ushort)Mathf.FloorToInt(f*ushort.MaxValue);
            return u/(float) ushort.MaxValue;
        }

        public static float RoundToUshort(float f)
        {
            f = Mathf.Clamp01(f);
            ushort u = (ushort)Mathf.RoundToInt(f * ushort.MaxValue);
            return u / (float)ushort.MaxValue;
        }

        public static T[,] Flip<T>(this T[,] array)
        {
            var result = new T[array.GetLength(1), array.GetLength(0)];
            for (int u = 0; u < array.GetLength(0); u++)
            {
                for (int v = 0; v < array.GetLength(1); v++)
                {
                    result[v, u] = array[u, v];
                }
            }
            return result;
        }

        public static void ConvertToIntArray(this byte[] inData, ref int[] outData)
        {
            if (inData.Length < outData.Length)
            {
                throw new Exception("Array too small!");
            }
            for (var i = 0; i < inData.Length; ++i)
            {
                outData[i] = (int)inData[i];
            }
        }

        public static int[] ConvertToIntArray(this byte[] inData)
        {
            var outData = new int[inData.Length];
            for (var i = 0; i < inData.Length; ++i)
            {
                outData[i] = (int)inData[i];
            }
            return outData;
        }
        
        public static void ProgressBar(string header, string text, float val)
        {
#if UNITY_EDITOR
            EditorUtility.DisplayProgressBar(header, text, val);
#endif
        }

        public static void ClearProgressBar()
        {
#if UNITY_EDITOR
            EditorUtility.ClearProgressBar();
#endif
        }

        public static Vector3 GetAveragePosition(this IEnumerable<MonoBehaviour> monoBehaviours)
        {
            Vector3? vec = null;
            int avg = 0;
            foreach (var monoBehaviour in monoBehaviours)
            {
                if (monoBehaviour == null || monoBehaviour.Equals(null))
                {
                    continue;
                }
                if (vec == null)
                {
                    vec = monoBehaviour.transform.position;
                }
                else
                {
                    vec += monoBehaviour.transform.position;
                }
                avg++;
            }
            return vec/(float)avg ?? Vector3.zero;
        }

        public static void Normalize(this float[, ,] array)
        {
            for (var i = 0; i < array.GetLength(0); ++i)
            {
                for (var j = 0; j < array.GetLength(1); ++j)
                {
                    float sum = 0;
                    for (var k = 0; k < array.GetLength(2); ++k)
                    {
                        sum += array[i, j, k];
                    }
                    if (Mathf.Approximately(sum, 0))
                    {
                        array[i, j, 0] = 1;
                        continue;
                    }
                    for (var k = 0; k < array.GetLength(2); ++k)
                    {
                        array[i, j, k] = array[i, j, k] / sum;
                    }
                }
            }
        }

    }
}