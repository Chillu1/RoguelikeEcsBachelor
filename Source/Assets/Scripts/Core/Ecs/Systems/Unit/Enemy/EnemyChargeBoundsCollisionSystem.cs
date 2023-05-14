using System;
using DefaultEcs;
using DefaultEcs.System;

namespace RoguelikeEcs.Core
{
	[With(typeof(IsChargeEnemy))]
	public sealed class EnemyChargeBoundsCollisionSystem : AEntitySetSystem<float>
	{
		private readonly Bounds _bounds;

		public EnemyChargeBoundsCollisionSystem(World world, Bounds bounds) : base(world)
		{
			_bounds = bounds;
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var enemy = ref entities[i];
				ref readonly var position = ref enemy.Get<PositionComp>();

				if (position.Value.x < _bounds.Min.x || position.Value.x > _bounds.Max.x ||
				    position.Value.y < _bounds.Min.y || position.Value.y > _bounds.Max.y)
				{
					enemy.Get<ChargeTimerComp>().Reset();
					enemy.Get<VelocityComp>().Speed = 100;
				}
			}
		}
	}
}