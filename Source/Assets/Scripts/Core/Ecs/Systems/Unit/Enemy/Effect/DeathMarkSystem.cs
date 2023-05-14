using System;
using DefaultEcs;
using DefaultEcs.System;

namespace RoguelikeEcs.Core
{
	[With(typeof(DeathMarkComp))]
	public sealed class DeathMarkSystem : AEntitySetSystem<float>
	{
		public DeathMarkSystem(World world) : base(world)
		{
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];
				ref var deathMark = ref entity.Get<DeathMarkComp>();

				if (deathMark.Duration <= 0)
					continue;

				deathMark.Duration -= state;

				if (deathMark.Duration <= 0)
					entity.Set<DestroyComp>();
			}
		}
	}
}