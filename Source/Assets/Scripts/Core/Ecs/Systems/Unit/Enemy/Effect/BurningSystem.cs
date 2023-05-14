using System;
using DefaultEcs;
using DefaultEcs.System;

namespace RoguelikeEcs.Core
{
	[With(typeof(BurningComp))]
	public sealed class BurningSystem : CustomEntitySetSystem
	{
		public BurningSystem(World world) : base(world, SystemParameters.UseBuffer)
		{
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			//TODO
			//float damageMultiplier = World.Get<UpgradesComp>().ElementalDamageMultiplier;

			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];

				ref var burning = ref entity.Get<BurningComp>();
				burning.Timer += state;

				if (burning.Timer < 1)
					continue;

				burning.Timer = 0;
				entity.Get<DamageEventsComp>().DamageEvents.Add(new DamageEvent
					{ Source = Player, Damage = burning.Damage, DamageType = DamageType.Fire });

				burning.Duration--;
				if (burning.Duration <= 0)
					entity.Remove<BurningComp>();
			}
		}
	}
}