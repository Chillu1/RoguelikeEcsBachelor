using System;
using DefaultEcs;
using DefaultEcs.System;

namespace RoguelikeEcs.Core
{
	[With(typeof(ColdComp))]
	public sealed class ColdSystem : AEntitySetSystem<float>
	{
		public ColdSystem(World world) : base(world, true)
		{
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];

				//Apply slow on init, remove slow after duration
				ref var cold = ref entity.Get<ColdComp>();
				cold.Timer += state;

				if (cold.Timer < cold.Duration)
					continue;

				//We'll have a problem if the cold percent resistance changes during the duration
				float coldMultiplier = entity.Get<ResistancesComp>().ColdPercent;

				entity.Get<MultiplierComp>().MultipliersList.Add(new MultiplierData()
				{
					StatType = StatType.MoveSpeed, Multiplier = 1f / (cold.SlowMultiplier * coldMultiplier)
				});

				entity.Remove<ColdComp>();
			}
		}
	}
}