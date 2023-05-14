using System.Diagnostics;
using UnityEngine;

namespace RoguelikeEcs.Benchmark.UNITY_EC_Follow
{
	public class CoreSystem : IFollowBenchmarkController
	{
		public int GetEntityCount() => _enemyCount;
		public Vector2 GetPlayerPosition() => _player.transform.position;
		public Vector2 GetEnemyPosition() => _enemyParent.GetChild(0).position;
		public Vector2 GetEnemyVelocity() => _enemyParent.GetChild(0).GetComponent<EnemyFollow>().Velocity;

		private int _enemyCount;

		private readonly MovingAverage _averageTime;

		private readonly Transform _enemyParent;
		private readonly Stopwatch _spawnStopWatch;

		private readonly GameObject _player;

		public CoreSystem(MovingAverage averageTime)
		{
			_averageTime = averageTime;
			_enemyParent = new GameObject("Enemies").transform;
			_spawnStopWatch = new Stopwatch();

			_player = new GameObject();
			_player.AddComponent<PlayerMovement>();
			_player.tag = "Player";
		}

		public void Update(float deltaTime)
		{
			_spawnStopWatch.Start();
			for (int i = 0; i < BenchmarkInitializer.EntityAddCount; i++)
			{
				GameObject enemy = new();
				enemy.AddComponent<EnemyFollow>();
				enemy.transform.parent = _enemyParent;
			}

			_spawnStopWatch.Stop();
			float sampleTime = deltaTime - (float)_spawnStopWatch.Elapsed.TotalSeconds; //Skipping game object creation time
			_spawnStopWatch.Reset();

			_enemyCount += BenchmarkInitializer.EntityAddCount;

			if (Time.time > 1)
				_averageTime.AddSample(sampleTime);
		}
	}
}