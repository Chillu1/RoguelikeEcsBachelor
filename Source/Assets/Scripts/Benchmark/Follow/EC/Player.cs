using UnityEngine;

namespace RoguelikeEcs.Benchmark.EC_Follow
{
	public class Player : IUpdatable
	{
		public readonly PositionComp Position;
		public readonly VelocityComp Velocity;

		public Player(PositionComp position)
		{
			Position = position;
			Velocity = new VelocityComp();
		}

		public void Update(float deltaTime)
		{
			float time = Time.time;
			Position.Value = new Vector2(Mathf.Sin(time) * 50, Mathf.Cos(time) * 50);
		}
	}
}