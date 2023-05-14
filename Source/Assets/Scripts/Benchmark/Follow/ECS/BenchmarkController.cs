using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;

namespace RoguelikeEcs.Benchmark.ECS_Follow
{
	public sealed class BenchmarkController : IFollowBenchmarkController
	{
		private readonly World _world;
		private readonly ISystem<float> _systems;

		public BenchmarkController(MovingAverage averageTime)
		{
			_world = new World();

			_systems = Core.GetCoreSystems(_world, averageTime);

			var player = EntityFactory.CreatePlayer(_world);
			player.Get<PositionComp>().Value = IFollowBenchmarkController.PlayerSpawnPosition;
		}

		public int GetEntityCount() => _world.GetEntities().AsSet().Count;
		public Vector2 GetPlayerPosition() => _world.GetEntities().With<IsPlayerComp>().AsSet().GetEntities()[0].Get<PositionComp>().Value;
		public Vector2 GetEnemyPosition() => _world.GetEntities().With<IsFollowerComp>().AsSet().GetEntities()[0].Get<PositionComp>().Value;
		public Vector2 GetEnemyVelocity() => _world.GetEntities().With<IsFollowerComp>().AsSet().GetEntities()[0].Get<VelocityComp>().Value;

		public void Update(float deltaTime)
		{
			_systems.Update(deltaTime);
		}
	}
}