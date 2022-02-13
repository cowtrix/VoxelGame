using UnityEngine;
using UnityEngine.Rendering;

public class AmbientLightingSettings
{
	public Color ambientEquatorColor;
	public Color ambientGroundColor;
	public float ambientIntensity;
	public Color ambientLight;
	public AmbientMode ambientMode;
	public SphericalHarmonicsL2 ambientProbe;
	public Color ambientSkyColor;
	public Texture customReflection;
	public DefaultReflectionMode defaultReflectionMode;
	public int defaultReflectionResolution;
	public float flareFadeSpeed;
	public float flareStrength;
	public bool fog;
	public Color fogColor;
	public float fogDensity;
	public float fogEndDistance;
	public float fogStartDistance;
	public FogMode fogMode;
	public float haloStrength;
	public int reflectionBounces;
	public Material skybox;
	public float reflectionIntensity;
	public Color subtractiveShadowColor;
	public Light sun;

	public static AmbientLightingSettings Current => new AmbientLightingSettings
	{
		ambientEquatorColor = RenderSettings.ambientEquatorColor,
		ambientGroundColor = RenderSettings.ambientGroundColor,
		ambientIntensity = RenderSettings.ambientIntensity,
		ambientLight = RenderSettings.ambientLight,
		ambientMode = RenderSettings.ambientMode,
		ambientProbe = RenderSettings.ambientProbe,
		ambientSkyColor = RenderSettings.ambientSkyColor,
		customReflection = RenderSettings.customReflection,
		defaultReflectionMode = RenderSettings.defaultReflectionMode,
		defaultReflectionResolution = RenderSettings.defaultReflectionResolution,
		flareFadeSpeed = RenderSettings.flareFadeSpeed,
		flareStrength = RenderSettings.flareStrength,
		fog = RenderSettings.fog,
		fogColor = RenderSettings.fogColor,
		fogDensity = RenderSettings.fogDensity,
		fogEndDistance = RenderSettings.fogEndDistance,
		fogStartDistance = RenderSettings.fogStartDistance,
		fogMode = RenderSettings.fogMode,
		haloStrength = RenderSettings.haloStrength,
		reflectionBounces = RenderSettings.reflectionBounces,
		reflectionIntensity = RenderSettings.reflectionIntensity,
		skybox = RenderSettings.skybox,
		subtractiveShadowColor = RenderSettings.subtractiveShadowColor,
		sun = RenderSettings.sun,
	};

	public void Apply()
	{
		RenderSettings.ambientEquatorColor = ambientEquatorColor;
		RenderSettings.ambientGroundColor = ambientGroundColor;
		RenderSettings.ambientIntensity = ambientIntensity;
		RenderSettings.ambientLight = ambientLight;
		RenderSettings.ambientMode = ambientMode;
		RenderSettings.ambientProbe = ambientProbe;
		RenderSettings.ambientSkyColor = ambientSkyColor;
		RenderSettings.customReflection = customReflection;
		RenderSettings.defaultReflectionMode = defaultReflectionMode;
		RenderSettings.defaultReflectionResolution = defaultReflectionResolution;
		RenderSettings.flareFadeSpeed = flareFadeSpeed;
		RenderSettings.flareStrength = flareStrength;
		RenderSettings.fog = fog;
		RenderSettings.fogColor = fogColor;
		RenderSettings.fogDensity = fogDensity;
		RenderSettings.fogEndDistance = fogEndDistance;
		RenderSettings.fogStartDistance = fogStartDistance;
		RenderSettings.fogMode = fogMode;
		RenderSettings.haloStrength = haloStrength;
		RenderSettings.reflectionBounces = reflectionBounces;
		RenderSettings.reflectionIntensity = reflectionIntensity;
		RenderSettings.skybox = skybox;
		RenderSettings.subtractiveShadowColor = subtractiveShadowColor;
		RenderSettings.sun = sun;
	}
}
