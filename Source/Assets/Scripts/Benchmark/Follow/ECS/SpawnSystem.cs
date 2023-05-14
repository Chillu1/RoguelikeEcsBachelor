using DefaultEcs;
using DefaultEcs.System;

namespace RoguelikeEcs.Benchmark.ECS_Follow
{
	internal sealed class SpawnSystem : ISystem<float>
	{
		public bool IsEnabled { get; set; } = true;

		private readonly World _world;

		public SpawnSystem(World world)
		{
			_world = world;

			EntityFactory.SetupFollowers(_world);
		}

		public void Update(float state)
		{
			for (int i = 0; i < BenchmarkInitializer.EntityAddCount; i++)
				EntityFactory.CreateFollower(_world);
		}

		public void Dispose()
		{
		}
	}
}