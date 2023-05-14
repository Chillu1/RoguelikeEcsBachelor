using System;
using DefaultEcs;
using DefaultEcs.System;

namespace RoguelikeEcs.Core
{
	[With(typeof(IsPinballEnemy))]
	public sealed class EnemyPinballBounds : CustomEntitySetSystem
	{
		private readonly Bounds _bounds;

		public EnemyPinballBounds(World world, Bounds bounds) : base(world, interval: 0.05f)
		{
			_bounds = bounds;
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var enemy = ref entities[i];

				ref readonly var position = ref enemy.Get<PositionComp>();
				ref var velocity = ref enemy.Get<VelocityComp>();

				if (position.Value.x < _bounds.Min.x && velocity.Direction.x < 0 ||
				    position.Value.x > _bounds.Max.x && velocity.Direction.x > 0)
					velocity.Direction.x = -velocity.Direction.x;

				if (position.Value.y < _bounds.Min.y && velocity.Direction.y < 0 ||
				    position.Value.y > _bounds.Max.y && velocity.Direction.y > 0)
					velocity.Direction.y = -velocity.Direction.y;
			}
		}
	}
}