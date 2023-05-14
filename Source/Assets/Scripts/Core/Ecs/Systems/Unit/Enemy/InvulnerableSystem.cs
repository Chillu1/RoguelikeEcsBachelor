using System;
using DefaultEcs;
using DefaultEcs.System;

namespace RoguelikeEcs.Core
{
	[With(typeof(InvulnerableComp))]
	public sealed class InvulnerableSystem : CustomEntitySetSystem
	{
		public InvulnerableSystem(World world) : base(world, SystemParameters.UseBuffer)
		{
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];

				ref var invulnerable = ref entity.Get<InvulnerableComp>();
				invulnerable.TimeLeft -= state;

				if (invulnerable.TimeLeft <= 0)
					entity.Remove<InvulnerableComp>();
			}
		}
	}
}