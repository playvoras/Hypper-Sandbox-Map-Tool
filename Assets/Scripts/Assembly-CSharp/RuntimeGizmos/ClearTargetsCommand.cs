using System.Collections.Generic;
using UnityEngine;

namespace RuntimeGizmos
{
	public class ClearTargetsCommand : SelectCommand
	{
		private List<Transform> targetRoots = new List<Transform>();

		public ClearTargetsCommand(TransformGizmo transformGizmo, List<Transform> targetRoots)
			: base(transformGizmo, null)
		{
			this.targetRoots.AddRange(targetRoots);
		}

		public override void Execute()
		{
			transformGizmo.ClearTargets(addCommand: false);
		}

		public override void UnExecute()
		{
			for (int i = 0; i < targetRoots.Count; i++)
			{
				transformGizmo.AddTarget(targetRoots[i], addCommand: false);
			}
		}
	}
}
