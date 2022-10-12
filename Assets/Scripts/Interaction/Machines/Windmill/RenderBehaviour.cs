using Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voxul.Utilities;

public abstract class RenderBehaviour : SlowUpdater
{
    public List<Renderer> Renderers = new List<Renderer>();
    public List<Canvas> Canvases = new List<Canvas>();
    public Bounds Bounds;
    public bool IsOnScreen { get; private set; }
    public Camera Camera => Camera.current;

    public void RecalculateBounds()
    {
        Bounds = default;
        if (!Renderers.Any())
        {
            Renderers = new List<Renderer>(GetComponentsInChildren<Renderer>());
        }
        if (Renderers.Any())
        {
            Bounds = Renderers.GetBounds();
        }
        Canvases = new List<Canvas>(GetComponentsInChildren<Canvas>());
        foreach(var canvas in Canvases)
        {
            var wCanvasBounds = canvas.GetBounds();
            var lCanvasBounds = GeometryExtensions.TranslateBounds(wCanvasBounds, transform.worldToLocalMatrix);
            Bounds.Encapsulate(lCanvasBounds);
        }
    }

    protected override int TickOnThread(float dt)
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
