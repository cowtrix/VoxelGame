using System;
using System.Collections.Generic;
using UnityEngine;
using Voxul;

[ExecuteAlways]
public class PropertyBlockSetter : ExtendedMonoBehaviour
{
    [Serializable]
    public class ColorRenderProperty
    {
        public string Name;
        public Color Color;
    }

    [Serializable]
    public class FloatRenderProperty
    {
        public string Name;
        public float Value;
    }

    [Serializable]
    public class Vector3RenderProperty
    {
        public string Name;
        public Vector3 Value;
    }

    public List<ColorRenderProperty> Colors = new List<ColorRenderProperty>();
    public List<FloatRenderProperty> Ints = new List<FloatRenderProperty>();
    public List<Vector3RenderProperty> Vector3s = new List<Vector3RenderProperty>();

    public AutoProperty<Renderer> Renderer;

    private MaterialPropertyBlock MaterialPropertyBlock;

    private void LateUpdate()
    {
        if(Renderer == null)
        {
            Renderer = new AutoProperty<Renderer>(gameObject, go => go.GetComponent<Renderer>());
        }
        if(MaterialPropertyBlock == null)
        {
            MaterialPropertyBlock = new MaterialPropertyBlock();
            Renderer.Value.GetPropertyBlock(MaterialPropertyBlock);
        }
        foreach(var c in Colors)
        {
            MaterialPropertyBlock.SetColor(c.Name, c.Color);
        }
        foreach(var k in Ints)
        {
            MaterialPropertyBlock.SetFloat(k.Name, k.Value);
        }
        foreach (var v in Vector3s)
        {
            MaterialPropertyBlock.SetVector(v.Name, v.Value);
        }
        Renderer.Value.SetPropertyBlock(MaterialPropertyBlock);
    }
}
