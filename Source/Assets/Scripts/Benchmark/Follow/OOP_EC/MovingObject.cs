using UnityEngine;

namespace RoguelikeEcs.Benchmark.OOP_EC_Follow
{
	public abstract class MovingObject : PositionObject
	{
		public Vector2 Velocity;

		protected void Move(float deltaTime)
		{
			Position += Velocity * deltaTime;
		}
	}
}