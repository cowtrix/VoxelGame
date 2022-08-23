using System;
using UnityEngine;

[Serializable]
public class AmbientLightingState
{
    [ColorUsage(false)]
    public Color AmbientSkyColor;
    [ColorUsage(false)]
    public Color AmbientEquatorColor;
    [ColorUsage(false)]
    public Color AmbientGroundColor;
    public float Intensity;

    public static AmbientLightingState Lerp(AmbientLightingState first, AmbientLightingState second, float t)
    {
        return new AmbientLightingState
        {
            AmbientSkyColor = Color.Lerp(first.AmbientSkyColor, second.AmbientSkyColor, t),
            AmbientEquatorColor = Color.Lerp(first.AmbientEquatorColor, second.AmbientEquatorColor, t),
            AmbientGroundColor = Color.Lerp(first.AmbientGroundColor, second.AmbientGroundColor, t),
            Intensity = Mathf.Lerp(first.Intensity, second.Intensity, t),
        };
    }

    public void Apply()
    {
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        RenderSettings.ambientGroundColor = AmbientGroundColor;
        RenderSettings.ambientEquatorColor = AmbientEquatorColor;
        RenderSettings.ambientSkyColor = AmbientSkyColor;
        RenderSettings.ambientIntensity = Intensity;
    }
}
