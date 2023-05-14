using System;
using DefaultEcs;
using DefaultEcs.System;

namespace RoguelikeEcs.Core
{
	[With(typeof(TimeOutComp))]
	public sealed class TimeOutSystem : AEntitySetSystem<float>
	{
		public TimeOutSystem(World world) : base(world)
		{
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			ref var score = ref World.Get<ScoreComp>();

			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];
				ref var timeOut = ref entity.Get<TimeOutComp>();

				if (timeOut.TimeLeft <= 0)
					continue;

				timeOut.TimeLeft -= state;

				if (timeOut.TimeLeft <= 0)
				{
					if (entity.Has<IsProjectileComp>() && entity.Get<PiercingComp>().EntitiesHit.Count == 0)
						score.MissedProjectiles++;

					entity.Set<DestroyComp>();
				}
			}
		}
	}
}