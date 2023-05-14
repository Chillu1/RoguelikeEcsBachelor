using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	[With(typeof(DamageEventsComp), typeof(IsPlayerComp))]
	public sealed class PlayerDamageSystem : AEntitySetSystem<float>
	{
		private bool _alwaysCrit;

		private float _critMultiplier;

		public PlayerDamageSystem(World world) : base(world)
		{
		}

		protected override void PreUpdate(float state)
		{
			ref readonly var upgradesComp = ref World.Get<EnemyUpgradesComp>();
			_alwaysCrit = upgradesComp.AlwaysCrit.IsOn();

			_critMultiplier = upgradesComp.CritMultiplier;
		}

		protected override void Update(float state, in Entity player)
		{
			var damageEvents = player.Get<DamageEventsComp>().DamageEvents;

			if (damageEvents.Count == 0)
				return;

			if (player.Has<InvulnerableComp>())
			{
				damageEvents.Clear();
				return;
			}

			ref var score = ref World.Get<ScoreComp>();
			ref readonly var resistances = ref player.Get<ResistancesComp>();

			ref var health = ref player.Get<HealthComp>();
			player.Get<PreviousHealthComp>().Value = health.Current;

			for (int j = 0; j < damageEvents.Count; j++)
			{
				var damageEvent = damageEvents[j];

				float volatileMultiplier = 1f;
				if (damageEvent.Source.TryGet(out MultiplierComp multiplierComp))
					volatileMultiplier = multiplierComp.BaseMultipliers[(int)StatType.Damage];

				ref var crit = ref damageEvent.Source.Get<CritComp>();
				if (_alwaysCrit || crit.Roll())
					volatileMultiplier *= crit.Multiplier * _critMultiplier;

				float damage = damageEvent.Damage * resistances.GetPercent(damageEvent.DamageType) * volatileMultiplier;

				health.Current -= damage;

				score.DamageTaken += damage;

				if (health.Current <= 0)
				{
					Debug.Log($"Player died, taking {damage} damage from entity: {damageEvent.Source}, " +
					          $"go: {damageEvent.Source.Get<GameObjectComp>().Value.name}. " +
					          $"IsProjectile: {damageEvent.Source.Has<IsProjectileComp>()}");
				}
			}

			damageEvents.Clear();
		}
	}
}