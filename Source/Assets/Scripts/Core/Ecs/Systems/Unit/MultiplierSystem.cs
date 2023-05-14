using System;
using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	[With(typeof(MultiplierComp))]
	public sealed class MultiplierSystem : AEntitySetSystem<float>
	{
		public MultiplierSystem(World world) : base(world)
		{
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];
				ref var multiplier = ref entity.Get<MultiplierComp>();
				if (multiplier.MultipliersList.Count == 0)
					continue;

				for (int j = 0; j < multiplier.MultipliersList.Count; j++)
				{
					var multiplierData = multiplier.MultipliersList[j];
					//if pool stat, update it
					if (multiplierData.StatType.IsPoolStat())
					{
						UpdatePoolStat(in entity, ref multiplier, in multiplierData);
						continue;
					}

					multiplier.BaseMultipliers[(int)multiplierData.StatType] *= multiplierData.Multiplier;
					//Debug.Log($"MultiplierSystem: {multiplierData.StatType} {multiplierData.Multiplier}");
				}

				multiplier.MultipliersList.Clear();
			}
		}

		private static void UpdatePoolStat(in Entity entity, ref MultiplierComp multiplier, in MultiplierData multiplierData)
		{
			float extra;
			ref var health = ref entity.Get<HealthComp>();
			float percent = health.Current / health.Max;
			float baseMaxHealth;
			if (entity.TryGet<StatsComp>(out var stats))
			{
				extra = stats.ExtraHealth;
				baseMaxHealth = PlayerSpawner.Health;
			}
			else
			{
				//Remove old multiplier (not ideal approach, but much better than applying the multiplier every time)
				health.Max /= multiplier.BaseMultipliers[(int)multiplierData.StatType];
				multiplier.BaseMultipliers[(int)multiplierData.StatType] *= multiplierData.Multiplier;
				health.Max *= multiplier.BaseMultipliers[(int)StatType.Health];
				health.Current = health.Max * percent;
				return;
			}

			//TODO Refactor
			multiplier.BaseMultipliers[(int)multiplierData.StatType] *= multiplierData.Multiplier;
			float newMaxHealth = (baseMaxHealth + extra) * multiplier.BaseMultipliers[(int)StatType.Health];
			health.Max = newMaxHealth;
			health.Current = newMaxHealth * percent;
		}
	}
}