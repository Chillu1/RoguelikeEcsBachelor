using DefaultEcs;
using UnityEngine;

namespace RoguelikeEcs.Benchmark.ECS_Follow
{
	internal static class EntityFactory
	{
		internal static Entity CreatePlayer(World world)
		{
			var entity = world.CreateEntity();

			entity.Set<IsPlayerComp>();
			entity.Set<PositionComp>();
			entity.Set<VelocityComp>();
			entity.Set<HealthComp>();

			return entity;
		}

		private static Entity _firstFollower;

		internal static void SetupFollowers(World world)
		{
			_firstFollower = world.CreateEntity();

			_firstFollower.Set<IsFollowerComp>();
			_firstFollower.Set(new PositionComp { Value = new Vector2(Random.Range(-100, 100), Random.Range(-100, 100)) });
			_firstFollower.Set<VelocityComp>();
			_firstFollower.Set<HealthComp>();
		}

		internal static Entity CreateFollower(World world)
		{
			var entity = _firstFollower.CopyTo(world);

			entity.Get<PositionComp>().Value = new Vector2(Random.Range(-100, 100), Random.Range(-100, 100));

			return entity;
		}
	}
}