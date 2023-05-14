using System;
using DefaultEcs;
using DefaultEcs.System;

namespace RoguelikeEcs.Core
{
	[With(typeof(IsEnemy), typeof(GrowComp))]
	public sealed class EnemyGrowSystem : CustomEntitySetSystem
	{
		public EnemyGrowSystem(World world) : base(world, SystemParameters.UseBuffer)
		{
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];
				ref var grow = ref entity.Get<GrowComp>();
				grow.Time -= state;

				if (grow.Time > 0)
					continue;

				World.Publish(new SpawnEnemyEvent(grow.EnemyType, entity.Get<PositionComp>().Value));
				entity.Remove<GrowComp>();
				entity.Set<DestroyComp>();
			}
		}
	}
}