using Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class RenderBehaviour : SlowUpdater
{
	public IEnumerable<Renderer> Renderers { get; private set; }
	public Bounds Bounds { get; private set; }
	public bool IsOnScreen { get; private set; }
	public Camera Camera => Camera.current;

	private void Start()
	{
		Renderers = GetComponentsInChildren<Renderer>().ToList();
		Bounds = Renderers.GetBounds();
	}

	protected override int Tick(float dt)
	{
		var screenRect = GeometryExtensions.WorldBoundsToScreenRect(Bounds, Camera);
		IsOnScreen = screenRect.ScreenRectIsOnScreen();
		return 1;
	}

	private void Update()
	{
		if (IsOnScreen)
		{
			UpdateOnScreen();
		}
	}

	protected abstract void UpdateOnScreen();
}
