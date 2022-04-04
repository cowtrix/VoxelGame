using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Common
{
	public static class QuaternionExtensions
	{
		public static Quaternion SnapToNearest90Degrees(this Quaternion quat)
		{
			var eulerAngles = quat.eulerAngles;
			eulerAngles.x = Mathf.Round(eulerAngles.x / 90) * 90;
			eulerAngles.y = Mathf.Round(eulerAngles.y / 90) * 90;
			eulerAngles.z = Mathf.Round(eulerAngles.z / 90) * 90;
			return Quaternion.Euler(eulerAngles);
		}
	}

	public static class RandomExtensions
	{
		public static bool Flip(this Random rand)
		{
			return rand.NextDouble() > 0.5;
		}
	}

	public static class GeometryExtensions
	{
		public static bool BetweenPlanes(Vector3 worldPos, Plane startPlane, Plane endPlane)
		{
			return startPlane.GetSide(worldPos) && endPlane.GetSide(worldPos);
		}

		public static IEnumerable<Vector3> AllPoints(this Bounds b)
		{
			yield return new Vector3(b.min.x, b.min.y, b.min.z);
			yield return new Vector3(b.min.x, b.min.y, b.max.z);
			yield return new Vector3(b.min.x, b.max.y, b.min.z);
			yield return new Vector3(b.min.x, b.max.y, b.max.z);

			yield return new Vector3(b.max.x, b.min.y, b.min.z);
			yield return new Vector3(b.max.x, b.min.y, b.max.z);
			yield return new Vector3(b.max.x, b.max.y, b.min.z);
			yield return new Vector3(b.max.x, b.max.y, b.max.z);
		}

		public static Rect WorldBoundsToScreenRect(this Bounds worldBounds, Camera camera)
		{
			if (!camera)
			{
				return default;
			}
			var screenRect = new Rect(camera.WorldToScreenPoint(worldBounds.center), Vector2.zero);
			foreach (var p in worldBounds.AllPoints())
			{
				var screenP = camera.WorldToScreenPoint(p);
				screenRect = screenRect.Encapsulate(screenP);
			}
			return screenRect;
		}

		public static Rect ScreenRectToViewportRect(this Rect rect) =>
			new Rect(rect.x / Screen.width, rect.y / Screen.height, rect.width / Screen.width, rect.height / Screen.height);

		public static bool ScreenRectIsOnScreen(this Rect rect) =>
			rect.Overlaps(new Rect(0, 0, Screen.width, Screen.height));

		public static Bounds GetEncompassingBounds(this IEnumerable<Bounds> enumerable)
		{
			if (enumerable == null || !enumerable.Any())
			{
				return default;
			}
			var b = enumerable.First();
			foreach (var b2 in enumerable.Skip(1))
			{
				b.Encapsulate(b2);
			}
			return b;
		}

		public static Rect ClipToScreen(this Rect rect)
		{
			var xMin = Mathf.Max(0, rect.xMin);
			var yMin = Mathf.Max(0, rect.yMin);

			var xMax = Mathf.Min(Screen.width, rect.xMax);
			var yMax = Mathf.Min(Screen.height, rect.yMax);

			return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
		}

		public static bool BoundsWithinFrustrum(this Camera camera, Bounds worldBounds)
		{
			foreach (var p in worldBounds.AllPoints())
			{
				var screenP = camera.WorldToScreenPoint(p);
				if(screenP.z > 0)
				{
					return true;
				}
			}
			return false;
		}

		public static Bounds GetBounds(this IEnumerable<Renderer> renderers)
		{
			if(renderers == null)
			{
				return default;
			}
			return renderers.Select(r => r.bounds).GetEncompassingBounds();
		}
	}

	public static class VectorExtensions
	{
		public static Vector3 Inverse(this Vector3 obj)
		{
			return new Vector3(1 / obj.x, 1 / obj.y, 1 / obj.z);
		}
		public static Vector3 Abs(this Vector3 obj)
		{
			return new Vector3(Mathf.Abs(obj.x), Mathf.Abs(obj.y), Mathf.Abs(obj.z));
		}
		public static Vector2 Abs(this Vector2 obj)
		{
			return new Vector3(Mathf.Abs(obj.x), Mathf.Abs(obj.y));
		}
		public static Vector2 xy(this Vector3 obj)
		{
			return new Vector2(obj.x, obj.y);
		}
		public static Vector3 xzy(this Vector3 obj)
		{
			return new Vector3(obj.x, obj.z, obj.y);
		}
		public static Vector3 zyx(this Vector3 obj)
		{
			return new Vector3(obj.z, obj.y, obj.x);
		}
		public static Vector2 yx(this Vector3 obj)
		{
			return new Vector2(obj.y, obj.x);
		}
		public static Vector2 zy(this Vector3 obj)
		{
			return new Vector2(obj.z, obj.y);
		}
		public static Vector2 xz(this Vector3 obj)
		{
			return new Vector2(obj.x, obj.z);
		}
		public static Vector2 zx(this Vector3 obj)
		{
			return new Vector2(obj.z, obj.x);
		}
		public static Vector2 yz(this Vector3 obj)
		{
			return new Vector2(obj.y, obj.z);
		}
		public static Vector3 ClampMagnitude(this Vector3 obj, float magnitude)
		{
			if (obj.sqrMagnitude > magnitude * magnitude)
			{
				return obj.normalized * magnitude;
			}
			return obj;
		}
		public static Vector3 Clamp(this Vector3 obj, Vector3 min, Vector3 max)
		{
			return new Vector3(Mathf.Clamp(obj.x, min.x, max.x), Mathf.Clamp(obj.y, min.y, max.y), Mathf.Clamp(obj.z, min.z, max.z));
		}
		public static Vector3 Round(this Vector3 vec)
		{
			return new Vector3(Mathf.Round(vec.x), Mathf.Round(vec.y), Mathf.Round(vec.z));
		}
		public static Vector3 Floor(this Vector3 vec)
		{
			return new Vector3(Mathf.Floor(vec.x), Mathf.Floor(vec.y), Mathf.Floor(vec.z));
		}
		public static Vector3 Ceil(this Vector3 vec)
		{
			return new Vector3(Mathf.Ceil(vec.x), Mathf.Ceil(vec.y), Mathf.Ceil(vec.z));
		}
		public static Vector3 ToRadian(this Vector3 obj)
		{
			return Mathf.Deg2Rad * obj;
		}
		public static Vector3 Clamp360Ranges(this Vector3 obj)
		{
			if (obj.x < -180)
			{
				obj.x += 360;
			}

			if (obj.x > 180)
			{
				obj.x -= 360;
			}

			if (obj.y < -180)
			{
				obj.y += 360;
			}

			if (obj.y > 180)
			{
				obj.y -= 360;
			}

			if (obj.z < -180)
			{
				obj.z += 360;
			}

			if (obj.z > 180)
			{
				obj.z -= 360;
			}

			return obj;
		}

		public static Vector4 ToVector4(this Vector3 v, float w)
		{
			return new Vector4(v.x, v.y, v.z, w);
		}

		public static Vector3 xyz(this Vector4 w)
		{
			return new Vector3(w.x * w.w, w.y * w.w, w.z * w.w);
		}

		public static Vector3 x0z(this Vector2 w, float y = 0)
		{
			return new Vector3(w.x, y, w.y);
		}

		public static Vector3 xy0(this Vector2 w, float z = 0)
		{
			return new Vector3(w.x, w.y, z);
		}

		public static Vector3 Flatten(this Vector3 obj)
		{
			return new Vector3(obj.x, 0, obj.z);
		}

		public static Vector3 RandomNormalized()
		{
			return new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
		}

		public static Vector3 RoundToIncrement(this Vector3 v, float snapValue)
		{
			return new Vector3
			(
				snapValue * Mathf.Round(v.x / snapValue),
				snapValue * Mathf.Round(v.y / snapValue),
				snapValue * Mathf.Round(v.z / snapValue)
			);
		}
		public static float ManhattenDistance(this Vector3 a, Vector3 b)
		{
			return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
		}
		public static bool IsOnAxis(this Vector3 vec)
		{
			vec = vec.normalized;
			return Mathf.Abs(Vector3.Dot(vec, Vector3.up)) == 1 ||
				Mathf.Abs(Vector3.Dot(vec, Vector3.right)) == 1 ||
				Mathf.Abs(Vector3.Dot(vec, Vector3.forward)) == 1;
		}

		public static Vector3 ClosestAxisNormal(this Vector3 vec)
		{
			var sign = new Vector3(Mathf.Sign(vec.x), Mathf.Sign(vec.y), Mathf.Sign(vec.z));
			vec = vec.Abs();
			return vec.x > vec.y ? (vec.x > vec.z ? Vector3.right * sign.x : Vector3.forward * sign.z)
				: (vec.y > vec.z ? Vector3.up * sign.y : Vector3.forward * sign.z);
		}
	}
}