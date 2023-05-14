using System;
using System.Diagnostics;
using DefaultEcs;
using DefaultEcs.Internal;
using DefaultEcs.System;
using UnityEngine;

namespace RoguelikeEcs.Benchmark.ECS_Follow
{
	[With(typeof(IsFollowerComp))]
	internal sealed class FollowSystem : AEntitySetSystem<float>
	{
		private readonly MovingAverage _averageTime;

		private readonly Stopwatch _stopwatch;
		private readonly EntitySet _playerEntitySet;

		private readonly ComponentPool<PositionComp> _positionPool;
		private readonly ComponentPool<VelocityComp> _velocityPool;

		public FollowSystem(World world, MovingAverage averageTime) : base(world)
		{
			_averageTime = averageTime;

			_stopwatch = new Stopwatch();
			_playerEntitySet = world.GetEntities().With<IsPlayerComp>().AsSet();

			_positionPool = ComponentManager<PositionComp>.GetOrCreate(World.WorldId);
			_velocityPool = ComponentManager<VelocityComp>.GetOrCreate(World.WorldId);
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			_stopwatch.Start();
			var playerPosition = _playerEntitySet.GetEntities()[0].Get<PositionComp>().Value;

			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];
				ref var position = ref _positionPool.Get(entity.EntityId); //entity.Get<PositionComp>();
				ref var velocity = ref _velocityPool.Get(entity.EntityId); //entity.Get<VelocityComp>();
				//TODO Randomized speeds
				velocity.Value = (playerPosition - position.Value).GetNormalized() * IFollowBenchmarkController.EnemySpeed;
				position.Value += velocity.Value * state;
			}

			//if (entities.Length > 0)
			//	Debug.Log("Follower position: " + entities[0].Get<PositionComp>().Value);

			_playerEntitySet.Complete();
			_stopwatch.Stop();
			if (Time.time > 1)
				_averageTime.AddSample((float)_stopwatch.Elapsed.TotalSeconds);

			_stopwatch.Reset();
		}
	}
}