using System;
using UnityEngine;

namespace RuntimeGizmos
{
	[Serializable]
	public class CustomTransformGizmos
	{
		public Transform xAxisGizmo;

		public Transform yAxisGizmo;

		public Transform zAxisGizmo;

		public Transform anyAxisGizmo;

		private Collider xAxisGizmoCollider;

		private Collider yAxisGizmoCollider;

		private Collider zAxisGizmoCollider;

		private Collider anyAxisGizmoCollider;

		private Vector3 originalXAxisScale;

		private Vector3 originalYAxisScale;

		private Vector3 originalZAxisScale;

		private Vector3 originalAnyAxisScale;

		public void Init(int layer)
		{
			if (xAxisGizmo != null)
			{
				SetLayerRecursively(xAxisGizmo.gameObject, layer);
				xAxisGizmoCollider = xAxisGizmo.GetComponentInChildren<Collider>();
				originalXAxisScale = xAxisGizmo.localScale;
			}
			if (yAxisGizmo != null)
			{
				SetLayerRecursively(yAxisGizmo.gameObject, layer);
				yAxisGizmoCollider = yAxisGizmo.GetComponentInChildren<Collider>();
				originalYAxisScale = yAxisGizmo.localScale;
			}
			if (zAxisGizmo != null)
			{
				SetLayerRecursively(zAxisGizmo.gameObject, layer);
				zAxisGizmoCollider = zAxisGizmo.GetComponentInChildren<Collider>();
				originalZAxisScale = zAxisGizmo.localScale;
			}
			if (anyAxisGizmo != null)
			{
				SetLayerRecursively(anyAxisGizmo.gameObject, layer);
				anyAxisGizmoCollider = anyAxisGizmo.GetComponentInChildren<Collider>();
				originalAnyAxisScale = anyAxisGizmo.localScale;
			}
		}

		public void SetEnable(bool enable)
		{
			if (xAxisGizmo != null && xAxisGizmo.gameObject.activeSelf != enable)
			{
				xAxisGizmo.gameObject.SetActive(enable);
			}
			if (yAxisGizmo != null && yAxisGizmo.gameObject.activeSelf != enable)
			{
				yAxisGizmo.gameObject.SetActive(enable);
			}
			if (zAxisGizmo != null && zAxisGizmo.gameObject.activeSelf != enable)
			{
				zAxisGizmo.gameObject.SetActive(enable);
			}
			if (anyAxisGizmo != null && anyAxisGizmo.gameObject.activeSelf != enable)
			{
				anyAxisGizmo.gameObject.SetActive(enable);
			}
		}

		public void SetAxis(AxisInfo axisInfo)
		{
			Quaternion rotation = Quaternion.LookRotation(axisInfo.zDirection, axisInfo.yDirection);
			if (xAxisGizmo != null)
			{
				xAxisGizmo.rotation = rotation;
			}
			if (yAxisGizmo != null)
			{
				yAxisGizmo.rotation = rotation;
			}
			if (zAxisGizmo != null)
			{
				zAxisGizmo.rotation = rotation;
			}
			if (anyAxisGizmo != null)
			{
				anyAxisGizmo.rotation = rotation;
			}
		}

		public void SetPosition(Vector3 position)
		{
			if (xAxisGizmo != null)
			{
				xAxisGizmo.position = position;
			}
			if (yAxisGizmo != null)
			{
				yAxisGizmo.position = position;
			}
			if (zAxisGizmo != null)
			{
				zAxisGizmo.position = position;
			}
			if (anyAxisGizmo != null)
			{
				anyAxisGizmo.position = position;
			}
		}

		public void ScaleMultiply(Vector4 scaleMultiplier)
		{
			if (xAxisGizmo != null)
			{
				xAxisGizmo.localScale = Vector3.Scale(originalXAxisScale, new Vector3(scaleMultiplier.w + scaleMultiplier.x, scaleMultiplier.w, scaleMultiplier.w));
			}
			if (yAxisGizmo != null)
			{
				yAxisGizmo.localScale = Vector3.Scale(originalYAxisScale, new Vector3(scaleMultiplier.w, scaleMultiplier.w + scaleMultiplier.y, scaleMultiplier.w));
			}
			if (zAxisGizmo != null)
			{
				zAxisGizmo.localScale = Vector3.Scale(originalZAxisScale, new Vector3(scaleMultiplier.w, scaleMultiplier.w, scaleMultiplier.w + scaleMultiplier.z));
			}
			if (anyAxisGizmo != null)
			{
				anyAxisGizmo.localScale = originalAnyAxisScale * scaleMultiplier.w;
			}
		}

		public Axis GetSelectedAxis(Collider selectedCollider)
		{
			if (xAxisGizmoCollider != null && xAxisGizmoCollider == selectedCollider)
			{
				return Axis.X;
			}
			if (yAxisGizmoCollider != null && yAxisGizmoCollider == selectedCollider)
			{
				return Axis.Y;
			}
			if (zAxisGizmoCollider != null && zAxisGizmoCollider == selectedCollider)
			{
				return Axis.Z;
			}
			if (anyAxisGizmoCollider != null && anyAxisGizmoCollider == selectedCollider)
			{
				return Axis.Any;
			}
			return Axis.None;
		}

		private void SetLayerRecursively(GameObject gameObject, int layer)
		{
			Transform[] componentsInChildren = gameObject.GetComponentsInChildren<Transform>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].gameObject.layer = layer;
			}
		}
	}
}
