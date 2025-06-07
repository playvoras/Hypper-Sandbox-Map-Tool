using UnityEngine;

namespace RuntimeGizmos
{
	public static class ExtTransform
	{
		public static void SetScaleFrom(this Transform target, Vector3 worldPivot, Vector3 newScale)
		{
			Vector3 vector = target.InverseTransformPoint(worldPivot);
			Vector3 localScale = target.localScale;
			Vector3 b = new Vector3(ExtMathf.SafeDivide(newScale.x, localScale.x), ExtMathf.SafeDivide(newScale.y, localScale.y), ExtMathf.SafeDivide(newScale.z, localScale.z));
			Vector3 vector2 = Vector3.Scale(vector, b);
			Vector3 position = target.TransformPoint(vector - vector2);
			target.localScale = newScale;
			target.position = position;
		}

		public static void SetScaleFromOffset(this Transform target, Vector3 worldPivot, Vector3 newScale)
		{
			Vector3 vector = target.InverseTransformPoint(worldPivot);
			Vector3 localScale = target.localScale;
			Vector3 b = new Vector3(ExtMathf.SafeDivide(newScale.x, localScale.x), ExtMathf.SafeDivide(newScale.y, localScale.y), ExtMathf.SafeDivide(newScale.z, localScale.z));
			Vector3 vector2 = Vector3.Scale(vector, b);
			Vector3 position = target.rotation * Vector3.Scale(vector - vector2, target.lossyScale) + target.position;
			target.localScale = newScale;
			target.position = position;
		}

		public static Vector3 GetCenter(this Transform transform, CenterType centerType)
		{
			switch (centerType)
			{
			case CenterType.Solo:
			{
				Renderer component = transform.GetComponent<Renderer>();
				if (component != null)
				{
					return component.bounds.center;
				}
				return transform.position;
			}
			case CenterType.All:
			{
				Bounds currentTotalBounds = new Bounds(transform.position, Vector3.zero);
				transform.GetCenterAll(ref currentTotalBounds);
				return currentTotalBounds.center;
			}
			default:
				return transform.position;
			}
		}

		private static void GetCenterAll(this Transform transform, ref Bounds currentTotalBounds)
		{
			Renderer component = transform.GetComponent<Renderer>();
			if (component != null)
			{
				currentTotalBounds.Encapsulate(component.bounds);
			}
			else
			{
				currentTotalBounds.Encapsulate(transform.position);
			}
			for (int i = 0; i < transform.childCount; i++)
			{
				transform.GetChild(i).GetCenterAll(ref currentTotalBounds);
			}
		}
	}
}
