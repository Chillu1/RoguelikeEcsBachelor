using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RoguelikeEcs.Benchmark.EC_Time
{
	public sealed class CoreSystem : IBenchmarkController
	{
		public int GetEntityCount() => _intervalCount;

		private readonly MovingAverage _averageTime;

		private readonly Stopwatch _stopwatch;

		private IntervalStructComponent[] _intervals;

		private int _intervalCount;

		public CoreSystem(MovingAverage averageTime)
		{
			_averageTime = averageTime;

			_stopwatch = new Stopwatch();

			_intervals = new IntervalStructComponent[(int)10e6];
		}

		public void Update(float deltaTime)
		{
			if (_intervalCount + BenchmarkInitializer.EntityAddCount > _intervals.Length)
				Array.Resize(ref _intervals, _intervals.Length * 2);
			for (int i = 0; i < BenchmarkInitializer.EntityAddCount; i++)
			{
				//_ = new IntervalClassComponent(); //Padding class fix (production scenario)
				//_ = new IntervalClassComponent();
				_intervals[_intervalCount + i] = new IntervalStructComponent();
				//_ = new IntervalClassComponent();
				//_ = new IntervalClassComponent();
			}

			_intervalCount += BenchmarkInitializer.EntityAddCount;

			_stopwatch.Start();
			//Parallel.For(0, _intervalCount, new ParallelOptions() { MaxDegreeOfParallelism = 4 }, i => _intervals[i].Update(deltaTime));
			for (int i = 0; i < _intervalCount; i++)
				_intervals[i].Update(deltaTime);

			_stopwatch.Stop();
			_averageTime.AddSample((float)_stopwatch.Elapsed.TotalSeconds);
			_stopwatch.Reset();
		}
	}
}