using System;
using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RoguelikeEcs.Core
{
	[WhenAdded(typeof(DestinationPositionComp))]
	public sealed class SetInitialDestinationSystem : AEntitySetSystem<float>
	{
		private readonly Bounds _bounds;

		public SetInitialDestinationSystem(World world, Bounds bounds) : base(world)
		{
			_bounds = bounds;
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];
				var destination = new Vector2(Random.Range(_bounds.Min.x, _bounds.Max.x), Random.Range(_bounds.Min.y, _bounds.Max.y));

				entity.Get<DestinationPositionComp>().Value = destination;
				entity.Get<VelocityComp>().Direction = (destination - entity.Get<PositionComp>().Value).normalized;
			}
		}
	}
}