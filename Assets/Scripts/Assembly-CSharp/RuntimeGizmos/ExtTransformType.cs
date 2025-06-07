namespace RuntimeGizmos
{
	public static class ExtTransformType
	{
		public static bool TransformTypeContains(this TransformType mainType, TransformType type, TransformSpace space)
		{
			if (type == mainType)
			{
				return true;
			}
			if (mainType == TransformType.All)
			{
				switch (type)
				{
				case TransformType.Move:
					return true;
				case TransformType.Rotate:
					return true;
				case TransformType.Scale:
					if (space == TransformSpace.Local)
					{
						return true;
					}
					break;
				}
			}
			return false;
		}
	}
}
