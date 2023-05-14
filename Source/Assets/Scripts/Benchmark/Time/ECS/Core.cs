using System.Diagnostics;
using DefaultEcs;
using DefaultEcs.System;
using DefaultEcs.Threading;

namespace RoguelikeEcs.Benchmark.ECS_Time
{
	internal static class Core
	{
		public static ISystem<float> GetCoreSystems(World world, MovingAverage movingAverage)
		{
			var stopWatch = new Stopwatch();
			return new SequentialSystem<float>(
				new SpawnSystem(world),
				new ActionSystem<float>(_ => stopWatch.Start()),
				new TimeSystem(world), //, new DefaultParallelRunner(4)),
				new ActionSystem<float>(_ =>
				{
					stopWatch.Stop();
					movingAverage.AddSample((float)stopWatch.Elapsed.TotalSeconds);
					stopWatch.Reset();
				})
			);
		}
	}
}