using Splines;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Cable))]
public class CableEditor : Editor
{
	protected Cable Cable => target as Cable;


	private void SceneGUIForControlPoint(SplineSegment.ControlPoint controlPoint)
	{
		switch (Tools.current)
		{
			case Tool.Move:
				controlPoint.Position = Handles.PositionHandle(controlPoint.Position, Quaternion.identity);
				break;
		}
		Handles.DrawLine(controlPoint.Position, controlPoint.Position + controlPoint.Control * Cable.Curviness);
	}

	private void OnSceneGUI()
	{
		var cable = Cable;
		SplineSegment.ControlPoint lastPoint = null;
		Tools.hidden = true;
		
		for (int i = cable.Spline.Count - 1; i >= 0; i--)
		{
			SplineSegment segment = cable.Spline[i];
			SceneGUIForControlPoint(segment.FirstControlPoint);
			if (i == cable.Spline.Count - 1)
			{
				SceneGUIForControlPoint(segment.SecondControlPoint);
				segment.SecondControlPoint.Control = Vector3.zero;
			}
			else
			{
				segment.SecondControlPoint = lastPoint;

				var dist = (lastPoint.Position - segment.SecondControlPoint.Position).magnitude / 2;
				var tangent = Vector3.ClampMagnitude(lastPoint.Tangent * cable.Curviness, dist);
				if (tangent != Vector3.zero)
				{
					segment.SecondControlPoint.Control = -tangent * cable.Curviness;
					lastPoint.Control = tangent * cable.Curviness;
				}

				Handles.DrawLine(segment.SecondControlPoint.Position, segment.SecondControlPoint.Position + segment.SecondControlPoint.Control * Cable.Curviness);
			}
			lastPoint = segment.FirstControlPoint;
		}

		if (cable.Spline.Any(s => s.IsDirty()))
		{
			EditorUtility.SetDirty(Cable);
			Cable.Invalidate();
		}
	}
}
