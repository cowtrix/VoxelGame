using System;
using System.Collections.Generic;
using UnityEngine;
using Voxul;

namespace Generation
{

    public class DynamicGeneratorColorTintNode : DynamicGeneratorNode
    {
        [Serializable]
        public class Tint
        {
            public VoxelColorTint Name;
            [Range(-1, 1)]
            public float HueShift;
            [Range(-1, 1)]
            public float SaturationShift;
            [Range(-1, 1)]
            public float ValueShift;
        }

        public List<Tint> Tints;

        public Vector2 Hue = new Vector2(0, 1);
        public Vector2 Saturation = new Vector2(0, 1);
        public Vector2 Value = new Vector2(0, 1);

        [ContextMenu("Collect Tints In Children")]
        public void CollectTints()
        {
            Tints.Clear();
            foreach(var child in GetComponentsInChildren<VoxelColorTint>())
            {
                Tints.Add(new Tint { Name = child });
            }
        }

        protected override void GenerateInternal()
        {
            UnityEngine.Random.InitState(Seed);

            var hue = UnityEngine.Random.Range(Hue.x, Hue.y);
            var sat = UnityEngine.Random.Range(Saturation.x, Saturation.y);
            var val = UnityEngine.Random.Range(Value.x, Value.y);

            foreach (var tint in Tints)
            {
                if (!tint.Name)
                {
                    continue;
                }
                tint.Name.Color = Color.HSVToRGB(Mathf.Clamp01(hue + tint.HueShift), Mathf.Clamp01(sat + tint.SaturationShift), Mathf.Clamp01(val + tint.ValueShift));
                tint.Name.Invalidate();
            }
        }
    }
}