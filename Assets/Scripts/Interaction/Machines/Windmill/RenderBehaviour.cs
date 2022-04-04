using Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class RenderBehaviour : SlowUpdater
{
	public List<Renderer> Renderers { get; private set; }
	public  virtual Bounds Bounds { get; private set; }
	public bool IsOnScreen { get; private set; }
	public Camera Camera => Camera.current;

	protected virtual void Start()
	{
		Renderers = GetComponentsInChildren<Renderer>().ToList();
		if (!Renderers.Any())
		{
			Renderers.Add(GetComponentInParent<Renderer>());
		}
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
