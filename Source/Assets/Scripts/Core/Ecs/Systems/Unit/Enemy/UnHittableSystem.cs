using System;
using DefaultEcs;
using DefaultEcs.System;

namespace RoguelikeEcs.Core
{
	[With(typeof(UnHittableComp))]
	public sealed class UnHittableSystem : CustomEntitySetSystem
	{
		public UnHittableSystem(World world) : base(world, SystemParameters.UseBuffer)
		{
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];

				ref var unHittable = ref entity.Get<UnHittableComp>();
				unHittable.TimeLeft -= state;

				if (unHittable.TimeLeft <= 0)
					entity.Remove<UnHittableComp>();
			}
		}
	}
}