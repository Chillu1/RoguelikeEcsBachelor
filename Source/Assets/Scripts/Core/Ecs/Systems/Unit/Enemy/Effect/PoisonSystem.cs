using System;
using DefaultEcs;
using DefaultEcs.System;

namespace RoguelikeEcs.Core
{
	[With(typeof(PoisonedComp))]
	public sealed class PoisonSystem : AEntitySetSystem<float>
	{
		private const int DefaultMaxStacks = 10;
		private int _maxStacks = DefaultMaxStacks;
		private const float DefaultTickInterval = 0.5f;
		
		private const float BleedComboMultiplier = 1.2f;
		public PoisonSystem(World world) : base(world)
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

				ref var poison = ref entity.Get<PoisonedComp>();
				poison.Timer -= state;

				if (poison.Timer <= 0)
				{
					entity.Remove<PoisonedComp>();
					continue;
				}

				if (poison.Stacks > _maxStacks)
					poison.Stacks = _maxStacks;

				var comboMultiplier = 1f;
				if (entity.Has<BleedingComp>())
					comboMultiplier = BleedComboMultiplier;

				poison.Interval -= state;
				if (poison.Interval <= 0)
				{
					entity.Get<HealthComp>().Current -= poison.Damage * poison.Stacks * comboMultiplier;
					poison.Interval = DefaultTickInterval;
				}
					
				
			}
		}
	}
}