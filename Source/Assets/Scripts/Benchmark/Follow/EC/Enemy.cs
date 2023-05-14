using UnityEngine;

namespace RoguelikeEcs.Benchmark.EC_Follow
{
	public class Enemy : IUpdatable
	{
		public readonly PositionComp Position;
		public readonly VelocityComp Velocity;
		private readonly PositionComp _target;

		public Enemy(PositionComp target)
		{
			_target = target;

			Position = new PositionComp() { Value = new Vector2(Random.Range(-100, 100), Random.Range(-100, 100)) };
			Velocity = new VelocityComp();
		}

		public void Update(float deltaTime)
		{
			Velocity.Value = (_target.Value - Position.Value).GetNormalized() * IFollowBenchmarkController.EnemySpeed;
			Position.Value += Velocity.Value * deltaTime;
		}
	}
}