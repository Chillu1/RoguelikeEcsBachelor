using DefaultEcs;
using DefaultEcs.System;

namespace RoguelikeEcs.Benchmark.ECS_Time
{
	internal class SpawnSystem : ISystem<float>
	{
		public bool IsEnabled { get; set; } = true;

		private readonly World _world;

		public SpawnSystem(World world)
		{
			_world = world;
		}

		public void Update(float state)
		{
			for (int i = 0; i < BenchmarkInitializer.EntityAddCount; i++)
				_world.CreateEntity().Set(new IntervalComp { Interval = 1f });
		}

		public void Dispose()
		{
		}
	}
}