using System;
using DefaultEcs;
using DefaultEcs.System;

namespace RoguelikeEcs.Core
{
	[WhenAdded(typeof(ColdComp))]
	public sealed class InitColdSystem : AEntitySetSystem<float>
	{
		public InitColdSystem(World world) : base(world)
		{
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];

				//We'll have a problem if the cold percent resistance changes during the duration
				float coldMultiplier = entity.Get<ResistancesComp>().ColdPercent;

				entity.Get<MultiplierComp>().MultipliersList.Add(new MultiplierData()
					{ StatType = StatType.MoveSpeed, Multiplier = entity.Get<ColdComp>().SlowMultiplier * coldMultiplier });
			}
		}
	}
}