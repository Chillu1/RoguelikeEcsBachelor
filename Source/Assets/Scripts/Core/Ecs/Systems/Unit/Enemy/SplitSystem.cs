using System;
using DefaultEcs;
using DefaultEcs.System;

namespace RoguelikeEcs.Core
{
	[With(typeof(SplitComp))]
	[WhenAdded(typeof(DestroyComp))]
	public sealed class SplitSystem : AEntitySetSystem<float>
	{
		public SplitSystem(World world) : base(world)
		{
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];
				ref var split = ref entity.Get<SplitComp>();
				split.SplitsLeft--;
				if (split.SplitsLeft < 0 || entity.Get<DestroyComp>().Internal)
					continue;

				World.Publish(new SpawnSplitEnemyEvent(entity));
			}
		}
	}
}