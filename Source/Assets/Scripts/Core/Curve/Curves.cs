using System.Collections.Generic;

namespace RoguelikeEcs.Core
{
	public static class Curves
	{
		public static readonly Curve DamageResistance = new Curve(new List<CurvePoint>()
		{
			new CurvePoint(0, 0.00f),
			new CurvePoint(100, 0.2f),
			new CurvePoint(1_000, 0.5f),
			new CurvePoint(10_000, 0.9f),
			new CurvePoint(20_000, 0.95f),
			new CurvePoint(50_000, 0.98f),
			new CurvePoint(100_000, 0.99f),
			new CurvePoint(1_000_000, 0.995f),
			new CurvePoint(10_000_000, 0.999f),
		}, negativeAllowed: true);

		public static readonly Curve AttackSpeed = new Curve(new List<CurvePoint>()
		{
			new CurvePoint(0, 0.5f),
			new CurvePoint(50, 0.2f),
			new CurvePoint(100, 0.1f),
			new CurvePoint(150, 0.02f),
		});

		public static readonly Curve CameraOrthographicSize = new Curve(new List<CurvePoint>()
		{
			new CurvePoint(200f, 260f),
			new CurvePoint(400f, 300f),
		});

		public static readonly Curve DamageToScore = new Curve(new List<CurvePoint>()
		{
			new CurvePoint(0, 0.0f),
			new CurvePoint(1_000, 40f),
			new CurvePoint(5_000, 60f),
			new CurvePoint(10_000, 80f),
			new CurvePoint(50_000, 150f),
			new CurvePoint(100_000, 200f),
		});

		public static readonly Curve KillStreakToScore = new Curve(new List<CurvePoint>()
		{
			new CurvePoint(0, 0.0f),
			new CurvePoint(50, 30f),
			new CurvePoint(250, 70f),
			new CurvePoint(500, 150f),
			new CurvePoint(1_000, 200f),
			new CurvePoint(5_000, 300f),
		});
	}
}