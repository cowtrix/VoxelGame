using Common;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Voxul;

public class WorldCanvas : SlowUpdater
{
    public float DisableDistance = 1000;
    public float RaycastDistance = 2;
    public Canvas Canvas => GetComponent<Canvas>();
    public GraphicRaycaster Raycaster => GetComponent<GraphicRaycaster>();
    public Camera Camera { get; private set; }

	protected override int Tick(float dt)
	{
		if (!Camera)
		{
            return 0;
		}
        var distance = Vector3.Distance(Camera.transform.position, transform.position);
        Canvas.enabled = distance < DisableDistance;
		if (Raycaster)
		{
            Raycaster.enabled = distance < RaycastDistance;
        }
        return 1;
    }

	protected void Awake()
    {
        Camera = Camera.main;
    }
}
