using System;
using DefaultEcs;
using DefaultEcs.System;
using RoguelikeEcs.Core;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	[With(typeof(HealthRegenComp))]
	[Without(typeof(IsPlayerComp))]
	public sealed class HealthRegenSystem : CustomEntitySetSystem
	{
		private float _regenAdd;
		private float _regenMultiplier;
		private float _negativeRegenMultiplier;

		//TODO RunWhenEmpty vs a new system for player mechanics
		public HealthRegenSystem(World world) : base(world, SystemParameters.UsePlayer | SystemParameters.RunWhenEmpty)
		{
		}

		protected override void PreUpdate(float state)
		{
			ref readonly var upgrades = ref World.Get<UpgradesComp>();
			_regenAdd = upgrades.RegenFlatAdd;
			_regenMultiplier = upgrades.RegenMultiplier;
			_negativeRegenMultiplier = upgrades.HealingNegative ? -1f : 1f;
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			ref var playerRegen = ref Player.Get<HealthRegenComp>();
			Heal(state, in Player, ref playerRegen, (playerRegen.Value + _regenAdd) * _regenMultiplier * _negativeRegenMultiplier);

			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];
				ref var regen = ref entity.Get<HealthRegenComp>();
				Heal(state, in entity, ref regen, regen.Value);
			}
		}

		private static void Heal(float state, in Entity entity, ref HealthRegenComp regen, float value) //TODO Profile static vs non-static
		{
			regen.Timer += state;

			if (regen.Timer < HealthRegenComp.Interval)
				return;

			regen.Timer = 0;
			ref var health = ref entity.Get<HealthComp>();
			health.Current = Mathf.Min(health.Current + value, health.Max);
		}
	}
}