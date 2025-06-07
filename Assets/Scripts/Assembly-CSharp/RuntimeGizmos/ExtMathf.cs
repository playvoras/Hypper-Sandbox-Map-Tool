namespace RuntimeGizmos
{
	public static class ExtMathf
	{
		public static float Squared(this float value)
		{
			return value * value;
		}

		public static float SafeDivide(float value, float divider)
		{
			if (divider == 0f)
			{
				return 0f;
			}
			return value / divider;
		}
	}
}
