using System;
using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	[With(typeof(IsPlayerComp), typeof(VelocityComp))]
	public sealed class PlayerMovementSystem : AEntitySetSystem<float>
	{
		private readonly Bounds _bounds;

		public PlayerMovementSystem(World world, Bounds bounds) : base(world)
		{
			_bounds = bounds;
		}

		protected override void Update(float delta, in Entity entity)
		{
			ref var position = ref entity.Get<PositionComp>();
			ref var velocity = ref entity.Get<VelocityComp>();
			float speed = (velocity.Speed + entity.Get<StatsComp>().ExtraMoveSpeed) *
			              entity.Get<MultiplierComp>().BaseMultipliers[(int)StatType.MoveSpeed];

			if (position.Value.x < _bounds.Min.x && velocity.Direction.x < 0 ||
			    position.Value.x > _bounds.Max.x && velocity.Direction.x > 0)
				velocity.Direction.x = 0;

			if (position.Value.y < _bounds.Min.y && velocity.Direction.y < 0 ||
			    position.Value.y > _bounds.Max.y && velocity.Direction.y > 0)
				velocity.Direction.y = 0;

			var rb = entity.Get<RigidbodyComp>().Value;

			if (velocity.Direction == Vector2.zero)
			{
				rb.velocity = Vector2.zero;
				return;
			}

			float desiredSpeed = Mathf.Lerp(rb.velocity.magnitude, speed, 0.3f);
			rb.velocity = desiredSpeed * velocity.Direction;
			World.Get<ScoreComp>().DistanceTraveled += desiredSpeed * delta;

			position.Value = rb.position;
		}
	}
}