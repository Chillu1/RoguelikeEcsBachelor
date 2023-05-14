using System;
using DefaultEcs;
using DefaultEcs.System;

namespace RoguelikeEcs.Core
{
	[With(typeof(PhaseComp))]
	public sealed class PhaseSystem : CustomEntitySetSystem
	{
		public PhaseSystem(World world) : base(world)
		{
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];
				ref var phase = ref entity.Get<PhaseComp>();
				phase.Timer += state;
				if (phase.Timer >= phase.ApplyInterval)
				{
					phase.Timer = 0;
					entity.Set(new InvulnerableComp() { TimeLeft = phase.Duration });
					entity.Set(new UnHittableComp() { TimeLeft = phase.Duration });
				}
			}
		}
	}
}