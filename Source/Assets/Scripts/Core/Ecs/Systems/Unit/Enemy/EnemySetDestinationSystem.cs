using System;
using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RoguelikeEcs.Core
{
	[With(typeof(IsEnemy), typeof(DestinationPositionComp))]
	public sealed class EnemySetDestinationSystem : AEntitySetSystem<float>
	{
		private readonly Bounds _bounds;

		public EnemySetDestinationSystem(World world, Bounds bounds) : base(world)
		{
			_bounds = bounds;
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];
				ref var destination = ref entity.Get<DestinationPositionComp>();
				ref readonly var position = ref entity.Get<PositionComp>();

				if (Vector2.Distance(position.Value, destination.Value) > 5f)
					continue;

				destination.Value = new Vector2(Random.Range(_bounds.Min.x, _bounds.Max.x), Random.Range(_bounds.Min.y, _bounds.Max.y));

				entity.Get<VelocityComp>().Direction = (destination.Value - position.Value).normalized;
			}
		}
	}
}