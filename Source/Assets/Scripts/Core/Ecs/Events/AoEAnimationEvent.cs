using UnityEngine;

namespace RoguelikeEcs.Core
{
	public readonly struct AoEAnimationEvent
	{
		public readonly AnimationType AnimationType;
		public readonly Vector2 Position;
		public readonly float Radius;

		public AoEAnimationEvent(AnimationType animationType, Vector2 position, float radius)
		{
			AnimationType = animationType;
			Position = position;
			Radius = radius;
		}
	}
}