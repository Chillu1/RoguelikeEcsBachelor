using UnityEngine;

namespace RoguelikeEcs.Core
{
	public sealed class Bounds
	{
		public Vector2 Min { get; }
		public Vector2 Max { get; }

		public Vector2 Size => Max - Min;
		public Vector2 Center => (Max + Min) / 2;

		public Vector2 ProjectileMin { get; }
		public Vector2 ProjectileMax { get; }

		public Bounds(Vector2 min, Vector2 max, Vector2 projectileMin, Vector2 projectileMax)
		{
			Min = min;
			Max = max;
			ProjectileMin = projectileMin;
			ProjectileMax = projectileMax;
		}
	}
}