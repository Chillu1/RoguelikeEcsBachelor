using DefaultEcs;
using DefaultEcs.System;
using System;
using DefaultEcs.Internal;

namespace RoguelikeEcs.Core
{
	[With(typeof(PiercingComp))]
	public class PiercingSystem : AEntitySetSystem<float>
	{
		private readonly ComponentPool<PiercingComp> _piercingPool;

		public PiercingSystem(World world) : base(world)
		{
			_piercingPool = ComponentManager<PiercingComp>.GetOrCreate(World.WorldId);
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];
				if (_piercingPool.Get(entity.EntityId).HitsLeft < 1)
					entity.Set<DestroyComp>();
			}
		}
	}
}