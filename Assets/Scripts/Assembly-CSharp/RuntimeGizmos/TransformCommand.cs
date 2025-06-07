using CommandUndoRedo;
using UnityEngine;

namespace RuntimeGizmos
{
	public class TransformCommand : ICommand
	{
		private struct TransformValues
		{
			public Vector3 position;

			public Quaternion rotation;
		}

		private TransformValues newValues;

		private TransformValues oldValues;

		private Transform transform;

		private TransformGizmo transformGizmo;

		public TransformCommand(TransformGizmo transformGizmo, Transform transform)
		{
			this.transformGizmo = transformGizmo;
			this.transform = transform;
			oldValues = new TransformValues
			{
				position = transform.position,
				rotation = transform.rotation
			};
		}

		public void StoreNewTransformValues()
		{
			newValues = new TransformValues
			{
				position = transform.position,
				rotation = transform.rotation
			};
		}

		public void Execute()
		{
			transform.position = newValues.position;
			transform.rotation = newValues.rotation;
			transformGizmo.SetPivotPoint();
		}

		public void UnExecute()
		{
			transform.position = oldValues.position;
			transform.rotation = oldValues.rotation;
			transformGizmo.SetPivotPoint();
		}
	}
}
