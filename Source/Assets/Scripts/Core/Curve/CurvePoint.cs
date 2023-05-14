namespace RoguelikeEcs.Core
{
	public readonly struct CurvePoint
	{
		public readonly float X;
		public readonly float Y;

		public CurvePoint(float x, float y)
		{
			X = x;
			Y = y;
		}

		public override string ToString()
		{
			return $"{X} {Y}";
		}
	}
}