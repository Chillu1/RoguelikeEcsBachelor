using DefaultEcs;
using DefaultEcs.System;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	[With(typeof(EnemyBuffComp))]
	public class EnemyBuffSystem : AEntitySetSystem<float>
	{
		private readonly List<BuffType> _buffsToRemove;

		public EnemyBuffSystem(World world) : base(world)
		{
			_buffsToRemove = new List<BuffType>(2);
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];
				var enemyBuffs = entity.Get<EnemyBuffComp>().Buffs;

				if (enemyBuffs.Count == 0)
					continue;

				foreach (var buff in enemyBuffs)
				{
					buff.Value.Time -= state;

					if (buff.Value.Time <= 0)
					{
						_buffsToRemove.Add(buff.Key);						

						switch (buff.Key)
						{
							case BuffType.HealthBuff:
								entity.Get<MultiplierComp>().MultipliersList.Add(new MultiplierData()
								{
									StatType = StatType.Health, Multiplier = 1f / 1.5f
								});
								break;
							case BuffType.SpeedBuff:
								entity.Get<MultiplierComp>().MultipliersList.Add(new MultiplierData()
								{
									StatType = StatType.MoveSpeed, Multiplier = 1f / 1.5f
								});
								break;
							case BuffType.DamageBuff:
								entity.Get<MultiplierComp>().MultipliersList.Add(new MultiplierData()
								{
									StatType = StatType.Damage, Multiplier = 1f / 1.5f
								});
								break;
							default:
								Debug.LogError("not implemented buff");
								break;
						}
					}
				}

				foreach (var buff in _buffsToRemove)
					enemyBuffs.Remove(buff);

				_buffsToRemove.Clear();
			}
		}
	}
}