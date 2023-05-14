using DefaultEcs;
using DefaultEcs.System;

namespace RoguelikeEcs.Benchmark.ECS_Follow
{
	public static class Core
	{
		public static ISystem<float> GetCoreSystems(World world, MovingAverage averageTime)
		{
			return new SequentialSystem<float>(
				new SpawnSystem(world),
				new PlayerMovementSystem(world),
				new FollowSystem(world, averageTime)
			);
		}
	}
}