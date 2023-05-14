using System.Diagnostics;
using UnityEngine;

namespace RoguelikeEcs.Benchmark.EC_Follow
{
	public sealed class CoreSystem : IFollowBenchmarkController
	{
		public int GetEntityCount() => _enemyCount;
		public Vector2 GetPlayerPosition() => _player.Position.Value;
		public Vector2 GetEnemyPosition() => _enemies[0].Position.Value;
		public Vector2 GetEnemyVelocity() => _enemies[0].Velocity.Value;

		private readonly MovingAverage _averageTime;

		private readonly Stopwatch _stopwatch;

		private readonly Player _player;
		private readonly Enemy[] _enemies;

		private int _enemyCount;

		public CoreSystem(MovingAverage averageTime)
		{
			_averageTime = averageTime;

			_stopwatch = new Stopwatch();

			_player = new Player(new PositionComp { Value = IFollowBenchmarkController.PlayerSpawnPosition });
			_enemies = new Enemy[(int)1e6];
		}

		public void Update(float deltaTime)
		{
			for (int i = 0; i < BenchmarkInitializer.EntityAddCount; i++)
				_enemies[_enemyCount + i] = new Enemy(_player.Position);
			_enemyCount += BenchmarkInitializer.EntityAddCount;

			_stopwatch.Start();
			_player.Update(deltaTime);
			for (int i = 0; i < _enemyCount; i++)
				_enemies[i].Update(deltaTime);

			_stopwatch.Stop();
			_averageTime.AddSample((float)_stopwatch.Elapsed.TotalSeconds);
			_stopwatch.Reset();
		}
	}
}