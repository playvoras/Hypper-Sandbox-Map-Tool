using System.Collections.Generic;
using UnityEngine;

namespace RuntimeGizmos
{
	public class AddTargetCommand : SelectCommand
	{
		private List<Transform> targetRoots = new List<Transform>();

		public AddTargetCommand(TransformGizmo transformGizmo, Transform target, List<Transform> targetRoots)
			: base(transformGizmo, target)
		{
			this.targetRoots.AddRange(targetRoots);
		}

		public override void Execute()
		{
			transformGizmo.AddTarget(target, addCommand: false);
		}

		public override void UnExecute()
		{
			transformGizmo.RemoveTarget(target, addCommand: false);
			for (int i = 0; i < targetRoots.Count; i++)
			{
				transformGizmo.AddTarget(targetRoots[i], addCommand: false);
			}
		}
	}
}
