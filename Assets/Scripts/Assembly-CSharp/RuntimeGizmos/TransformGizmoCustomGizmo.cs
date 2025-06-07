using System;
using UnityEngine;

namespace RuntimeGizmos
{
	public class TransformGizmoCustomGizmo : MonoBehaviour
	{
		public bool autoFindTransformGizmo = true;

		public TransformGizmo transformGizmo;

		public CustomTransformGizmos customTranslationGizmos = new CustomTransformGizmos();

		public CustomTransformGizmos customRotationGizmos = new CustomTransformGizmos();

		public CustomTransformGizmos customScaleGizmos = new CustomTransformGizmos();

		public bool scaleBasedOnDistance = true;

		public float scaleMultiplier = 0.4f;

		public int gizmoLayer = 2;

		private LayerMask mask;

		private void Awake()
		{
			if (transformGizmo == null && autoFindTransformGizmo)
			{
				transformGizmo = UnityEngine.Object.FindObjectOfType<TransformGizmo>();
			}
			transformGizmo.manuallyHandleGizmo = true;
			transformGizmo.circularRotationMethod = true;
			mask = LayerMask.GetMask(LayerMask.LayerToName(gizmoLayer));
			customTranslationGizmos.Init(gizmoLayer);
			customRotationGizmos.Init(gizmoLayer);
			customScaleGizmos.Init(gizmoLayer);
		}

		private void OnEnable()
		{
			TransformGizmo obj = transformGizmo;
			obj.onCheckForSelectedAxis = (Action)Delegate.Combine(obj.onCheckForSelectedAxis, new Action(CheckForSelectedAxis));
			TransformGizmo obj2 = transformGizmo;
			obj2.onDrawCustomGizmo = (Action)Delegate.Combine(obj2.onDrawCustomGizmo, new Action(OnDrawCustomGizmos));
		}

		private void OnDisable()
		{
			TransformGizmo obj = transformGizmo;
			obj.onCheckForSelectedAxis = (Action)Delegate.Remove(obj.onCheckForSelectedAxis, new Action(CheckForSelectedAxis));
			TransformGizmo obj2 = transformGizmo;
			obj2.onDrawCustomGizmo = (Action)Delegate.Remove(obj2.onDrawCustomGizmo, new Action(OnDrawCustomGizmos));
		}

		private void CheckForSelectedAxis()
		{
			ShowProperGizmoType();
			if (Input.GetMouseButtonDown(0) && Physics.Raycast(transformGizmo.myCamera.ScreenPointToRay(Input.mousePosition), out var hitInfo, float.PositiveInfinity, mask))
			{
				Axis axis = Axis.None;
				TransformType type = transformGizmo.transformType;
				if (axis == Axis.None && transformGizmo.TransformTypeContains(TransformType.Move))
				{
					axis = customTranslationGizmos.GetSelectedAxis(hitInfo.collider);
					type = TransformType.Move;
				}
				if (axis == Axis.None && transformGizmo.TransformTypeContains(TransformType.Rotate))
				{
					axis = customRotationGizmos.GetSelectedAxis(hitInfo.collider);
					type = TransformType.Rotate;
				}
				if (axis == Axis.None && transformGizmo.TransformTypeContains(TransformType.Scale))
				{
					axis = customScaleGizmos.GetSelectedAxis(hitInfo.collider);
					type = TransformType.Scale;
				}
				transformGizmo.SetTranslatingAxis(type, axis);
			}
		}

		private void OnDrawCustomGizmos()
		{
			if (transformGizmo.TranslatingTypeContains(TransformType.Move))
			{
				DrawCustomGizmo(customTranslationGizmos);
			}
			if (transformGizmo.TranslatingTypeContains(TransformType.Rotate))
			{
				DrawCustomGizmo(customRotationGizmos);
			}
			if (transformGizmo.TranslatingTypeContains(TransformType.Scale))
			{
				DrawCustomGizmo(customScaleGizmos);
			}
		}

		private void DrawCustomGizmo(CustomTransformGizmos customGizmo)
		{
			AxisInfo axisInfo = transformGizmo.GetAxisInfo();
			customGizmo.SetAxis(axisInfo);
			customGizmo.SetPosition(transformGizmo.pivotPoint);
			Vector4 one = Vector4.one;
			if (scaleBasedOnDistance)
			{
				one.w *= scaleMultiplier * transformGizmo.GetDistanceMultiplier();
			}
			if (transformGizmo.transformingType == TransformType.Scale)
			{
				float num = 1f + transformGizmo.totalScaleAmount;
				if (transformGizmo.translatingAxis == Axis.Any)
				{
					one += Vector4.one * num;
				}
				else if (transformGizmo.translatingAxis == Axis.X)
				{
					one.x *= num;
				}
				else if (transformGizmo.translatingAxis == Axis.Y)
				{
					one.y *= num;
				}
				else if (transformGizmo.translatingAxis == Axis.Z)
				{
					one.z *= num;
				}
			}
			customGizmo.ScaleMultiply(one);
		}

		private void ShowProperGizmoType()
		{
			bool flag = transformGizmo.mainTargetRoot != null;
			customTranslationGizmos.SetEnable(flag && transformGizmo.TranslatingTypeContains(TransformType.Move));
			customRotationGizmos.SetEnable(flag && transformGizmo.TranslatingTypeContains(TransformType.Rotate));
			customScaleGizmos.SetEnable(flag && transformGizmo.TranslatingTypeContains(TransformType.Scale));
		}
	}
}
