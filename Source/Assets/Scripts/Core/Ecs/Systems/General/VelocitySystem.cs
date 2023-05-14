using System;
using DefaultEcs;
using DefaultEcs.Internal;
using DefaultEcs.System;

namespace RoguelikeEcs.Core
{
	[With(typeof(VelocityComp))]
	[Without(typeof(IsPlayerComp))]
	public sealed class VelocitySystem : AEntitySetSystem<float>
	{
		private readonly ComponentPool<VelocityComp> _velocityPool;
		private readonly ComponentPool<MultiplierComp> _multiplierPool;
		private readonly ComponentPool<RigidbodyComp> _rigidbodyPool;
		private readonly ComponentPool<PositionComp> _positionPool;

		public VelocitySystem(World world) : base(world)
		{
			_velocityPool = ComponentManager<VelocityComp>.GetOrCreate(World.WorldId);
			_multiplierPool = ComponentManager<MultiplierComp>.GetOrCreate(World.WorldId);
			_rigidbodyPool = ComponentManager<RigidbodyComp>.GetOrCreate(World.WorldId);
			_positionPool = ComponentManager<PositionComp>.GetOrCreate(World.WorldId);
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			int length = entities.Length;
			for (int i = 0; i < length; i++)
			{
				ref readonly var entity = ref entities[i];
				ref readonly var velocity = ref _velocityPool.Get(entity.EntityId);
				float speed = velocity.Speed;

				if (_multiplierPool.TryGet(entity.EntityId, out var multiplier))
					speed *= multiplier.BaseMultipliers[(int)StatType.MoveSpeed];

				var rb = _rigidbodyPool.Get(entity.EntityId).Value;
				//rb.AddForce(new Vector2(velocity.X * speed * state, velocity.Y * speed * state) * 10, ForceMode2D.Force);
				rb.velocity = velocity.Direction * speed;
				//rb.MovePosition(rb.position + velocity.Direction * (speed * state));

				_positionPool.Get(entity.EntityId).Value = rb.position;
			}
		}
	}
}