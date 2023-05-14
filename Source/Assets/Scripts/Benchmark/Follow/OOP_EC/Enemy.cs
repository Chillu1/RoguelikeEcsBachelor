using UnityEngine;

namespace RoguelikeEcs.Benchmark.OOP_EC_Follow
{
	public sealed class Enemy : MovingObject
	{
		private readonly PositionObject _target;
		private readonly HealthObject _health;
		private readonly int _enemyPadding, _enemyPadding2, _enemyPadding3; //Enemy specific fields replacement that would be added later

		public Enemy(PositionObject target)
		{
			_target = target;
			_health = new HealthObject
			{
				Max = 100,
				Current = 100
			};
			Position = new Vector2(Random.Range(-100, 100), Random.Range(-100, 100));
		}

		public void Update(float deltaTime)
		{
			Velocity = (_target.Position - Position).GetNormalized() * IFollowBenchmarkController.EnemySpeed;
			Move(deltaTime);
		}
	}
}