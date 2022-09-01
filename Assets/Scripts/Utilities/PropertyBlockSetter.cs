using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
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
    public class VectorRenderProperty
    {
        public string Name;
        public Vector4 Value;
    }

    [Serializable]
    public class Vector2RenderProperty
    {
        public string Name;
        public Vector2 Value;
    }

    [Serializable]
    public class TextureRenderProperty
    {
        public string Name;
        public Texture Value;
    }

    public List<ColorRenderProperty> Colors = new List<ColorRenderProperty>();
    public List<FloatRenderProperty> Ints = new List<FloatRenderProperty>();
    [FormerlySerializedAs("Vector3s")]
    public List<VectorRenderProperty> Vectors = new List<VectorRenderProperty>();
    public List<TextureRenderProperty> Textures = new List<TextureRenderProperty>();

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
        }
        Renderer.Value.GetPropertyBlock(MaterialPropertyBlock);
        foreach (var c in Colors)
        {
            MaterialPropertyBlock.SetColor(c.Name, c.Color);
        }
        foreach(var k in Ints)
        {
            MaterialPropertyBlock.SetFloat(k.Name, k.Value);
        }
        foreach (var v in Vectors)
        {
            MaterialPropertyBlock.SetVector(v.Name, v.Value);
        }
        foreach (var v in Textures)
        {
            if (!v.Value)
            {
                continue;
            }
            MaterialPropertyBlock.SetTexture(v.Name, v.Value);
        }
        Renderer.Value.SetPropertyBlock(MaterialPropertyBlock);
    }
}
