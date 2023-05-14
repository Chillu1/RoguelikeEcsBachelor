using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;

namespace RoguelikeEcs.Benchmark.ECS_Time
{
	public sealed class CoreSystem : IBenchmarkController
	{
		private readonly World _world;

		private readonly ISystem<float> _systems;

		public CoreSystem(MovingAverage movingAverage)
		{
			_world = new World();

			_systems = Core.GetCoreSystems(_world, movingAverage);
		}

		public int GetEntityCount() => _world.GetEntities().AsSet().Count;

		public Vector2 GetPlayerPosition() => default;
		public Vector2 GetEnemyPosition() => default;
		public Vector2 GetEnemyVelocity() => default;

		public void Update(float deltaTime)
		{
			_systems.Update(deltaTime);
		}
	}
}