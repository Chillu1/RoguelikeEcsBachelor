using System;
using DefaultEcs;
using DefaultEcs.System;

namespace RoguelikeEcs.Core
{
	[With(typeof(BleedingComp))]
	public class BleedingSystem : AEntitySetSystem<float>
	{
		private const int DefaultMaxStacks = 5;
		private int _maxStacks = DefaultMaxStacks;
		private const float DefaultTickInterval = 1f;
		
		private const float BleedComboMultiplier = 1.2f;
		public BleedingSystem(World world) : base(world)
		{
		}
		
		protected override void PreUpdate(float state)
		{
			ref readonly var upgrades = ref World.Get<UpgradesComp>();
			if (upgrades.EffectsMoreStacks.IsOn())
				_maxStacks = DefaultMaxStacks * 10;
		}
		
		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];

				ref var bleed = ref entity.Get<BleedingComp>();
				bleed.Timer -= state;

				if (bleed.Timer <= 0)
				{
					entity.Remove<BleedingComp>();
					continue;
				}

				if (bleed.Stacks > _maxStacks)
					bleed.Stacks = _maxStacks;

				bleed.Interval -= state;
				if (bleed.Interval <= 0)
				{
					entity.Get<HealthComp>().Current -= bleed.Damage * bleed.Stacks;
					bleed.Interval = DefaultTickInterval;
				}
			}
		}
	}
}