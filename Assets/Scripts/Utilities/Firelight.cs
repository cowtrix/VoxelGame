using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firelight : MonoBehaviour
{
    public Gradient Gradient;
	public Vector2 Intensity = new Vector2(1, 2);
	public float FlickerSpeed = 1;
	public int Seed;
    public Light Light;

	protected void Update()
	{
		Light.color = Gradient.Evaluate(Perlin.Noise(Time.time * FlickerSpeed + Seed));
		Light.intensity = Mathf.Lerp(Intensity.x, Intensity.y, Perlin.Noise(Time.time * FlickerSpeed - Seed));
	}
}
