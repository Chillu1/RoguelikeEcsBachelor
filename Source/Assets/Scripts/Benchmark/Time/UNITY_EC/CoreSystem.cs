using System.Diagnostics;
using UnityEngine;

namespace RoguelikeEcs.Benchmark.UNITY_EC_Time
{
	public class CoreSystem : IBenchmarkController
	{
		public int GetEntityCount() => _intervalCount;

		private readonly MovingAverage _averageTime;

		private readonly Stopwatch _spawnStopWatch;

		private int _intervalCount;

		private readonly Transform _intervalParent;

		public CoreSystem(MovingAverage averageTime)
		{
			_averageTime = averageTime;

			_spawnStopWatch = new Stopwatch();
			_intervalParent = new GameObject("IntervalParent").transform;
		}

		public void Update(float deltaTime)
		{
			_spawnStopWatch.Start();
			for (int i = 0; i < BenchmarkInitializer.EntityAddCount; i++)
			{
				GameObject interval = new();
				interval.AddComponent<IntervalComponent>();
				interval.transform.parent = _intervalParent;
			}

			_intervalCount += BenchmarkInitializer.EntityAddCount;

			_spawnStopWatch.Stop();
			float sampleTime = deltaTime - (float)_spawnStopWatch.Elapsed.TotalSeconds; //Skipping game object creation time
			_spawnStopWatch.Reset();

			if (Time.time > 1)
				_averageTime.AddSample(sampleTime);
		}
	}
}