using UnityEngine;

namespace RoguelikeEcs.Core
{
	public readonly struct RangedAttackAnimationEvent
	{
		public readonly Vector2 Position;
		public readonly float Radius;
		public readonly float Angle;

		public RangedAttackAnimationEvent(Vector2 position, float radius, float angle)
		{
			Position = position;
			Radius = radius;
			Angle = angle;
		}
	}
}