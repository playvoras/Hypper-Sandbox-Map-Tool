using UnityEngine;

namespace RuntimeGizmos
{
	public class RemoveTargetCommand : SelectCommand
	{
		public RemoveTargetCommand(TransformGizmo transformGizmo, Transform target)
			: base(transformGizmo, target)
		{
		}

		public override void Execute()
		{
			transformGizmo.RemoveTarget(target, addCommand: false);
		}

		public override void UnExecute()
		{
			transformGizmo.AddTarget(target, addCommand: false);
		}
	}
}
