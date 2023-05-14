using System;
using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	[With(typeof(AttackComp))]
	[Without(typeof(IsSleeping))]
	public sealed class AttackSystem : CustomEntitySetSystem
	{
		private readonly int _attackSpeedIndex;

		public AttackSystem(World world) : base(world, SystemParameters.UsePlayer)
		{
			_attackSpeedIndex = (int)StatType.AttackSpeed;
		}

		protected override void PreUpdate(float state)
		{
			ref readonly var upgrades = ref World.Get<UpgradesComp>();
			ref readonly var stats = ref Player.Get<StatsComp>();
			Player.Get<AttackComp>().Cooldown = Curves.AttackSpeed.Evaluate(stats.ExtraAttackSpeed);
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];
				ref var attack = ref entity.Get<AttackComp>();
				if (attack.CanAttack)
					continue;

				attack.Timer += state;
				//Debug.Log(attack.Timer);
				if (attack.Timer * entity.Get<MultiplierComp>().BaseMultipliers[_attackSpeedIndex] < attack.Cooldown)
					continue;

				attack.CanAttack = true;
				attack.Timer = 0f;
			}
		}
	}
}