using Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class RenderBehaviour : SlowUpdater
{
    public List<Renderer> Renderers { get; private set; }
    public List<Canvas> Canvases { get; private set; }
    public virtual Bounds Bounds { get; private set; }
    public bool IsOnScreen { get; private set; }
    public Camera Camera => Camera.current;
    public bool IncludeManualBounds;
    public Bounds ManualBounds;

    protected virtual void Start()
    {
        Renderers = GetComponentsInChildren<Renderer>().ToList();
        if (!Renderers.Any())
        {
            Renderers.Add(GetComponentInParent<Renderer>());
        }
        if (Renderers.Any())
        {
            Bounds = Renderers.GetBounds();
        }
        if (IncludeManualBounds)
        {
            Bounds.Encapsulate(GeometryExtensions.TranslateBounds(ManualBounds, transform.localToWorldMatrix));
        }
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

    private void OnDrawGizmosSelected()
    {
        var translatedBounds = GeometryExtensions.TranslateBounds(ManualBounds, transform.localToWorldMatrix);
        if (IncludeManualBounds)
        {
            Gizmos.DrawWireCube(translatedBounds.center, translatedBounds.size);
        }
    }
}
