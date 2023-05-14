using System;
using System.Collections.Generic;
using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RoguelikeEcs.Core
{
	[With(typeof(DamageEventsComp), typeof(IsEnemy))]
	public sealed class DamageSystem : CustomEntitySetSystem
	{
		private float _totalPlayerDamageMultiplier;
		private float _damageMinMultiplier, _damageMidMultiplier, _damageMaxMultiplier;
		private float _fullHealthDamageMultiplier = 2f, _lowHealthDamageMultiplier = 2f;
		private float _lifestealMultiplier, _lifestealNegativeMultiplier;
		private float _critMultiplier;
		private float _flyingDamageMultiplier;
		private float _healOnAttackChance;
		private int _maxPierces;

		private bool _farDamageIncrease,
			_closeDamageIncrease,
			_targetFullHealthDamageIncrease,
			_targetLowHealthDamageIncrease,
			_selfFullHealthDamageIncrease,
			_bulletsNotLethal,
			_alwaysCrit,
			_cullingProjectiles;

		public DamageSystem(World world) : base(world, SystemParameters.UsePlayer)
		{
		}

		protected override void PreUpdate(float state)
		{
			_totalPlayerDamageMultiplier = Player.Get<MultiplierComp>().BaseMultipliers[(int)StatType.Damage];
			ref readonly var playerHealth = ref Player.Get<HealthComp>();
			ref readonly var upgrades = ref World.Get<UpgradesComp>();
			_farDamageIncrease = upgrades.HasFarDamage.IsOn();
			_closeDamageIncrease = upgrades.HasCloseDamage.IsOn();
			_targetFullHealthDamageIncrease = upgrades.HasFullHealthDamage.IsOn();
			_targetLowHealthDamageIncrease = upgrades.HasLowHealthDamage.IsOn();
			_bulletsNotLethal = upgrades.HasBulletsNotLethal.IsOn();
			_cullingProjectiles = upgrades.HasCullingProjectiles.IsOn();

			_damageMinMultiplier = _damageMidMultiplier = _damageMaxMultiplier = 1f;
			if (_farDamageIncrease)
			{
				_damageMaxMultiplier = 3f;
				if (!_closeDamageIncrease)
					_damageMinMultiplier = 0.5f;
			}

			if (_closeDamageIncrease)
			{
				_damageMinMultiplier = 3f;
				if (!_farDamageIncrease)
					_damageMaxMultiplier = 0.5f;
			}

			if (upgrades.HasPlayerFullHealthDamage.IsOn())
			{
				if (Math.Abs(playerHealth.Current - playerHealth.Max) < 0.1f)
					_totalPlayerDamageMultiplier *= 2f;
			}

			if (upgrades.HasPlayerLowHealthDamage.IsOn())
			{
				if (playerHealth.Current / playerHealth.Max < 0.25f)
					_totalPlayerDamageMultiplier *= 2f;
			}

			_flyingDamageMultiplier = upgrades.FlyingDamageMultiplier;

			_lifestealMultiplier = upgrades.LifestealMultiplier;
			_lifestealNegativeMultiplier = upgrades.LifestealNegative ? -1f : 1f;

			_critMultiplier = upgrades.CritMultiplier;

			_alwaysCrit = upgrades.AlwaysCrit.IsOn();

			_healOnAttackChance = upgrades.HealOnAttack.IsOn() ? upgrades.HealOnAttackChance : 0f;

			_maxPierces = upgrades.MaxPierces;
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			ref var playerHealth = ref Player.Get<HealthComp>();
			ref var score = ref World.Get<ScoreComp>();

			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];
				var damageEvents = entity.Get<DamageEventsComp>().DamageEvents;

				if (damageEvents.Count == 0)
					continue;

				if (entity.Has<InvulnerableComp>())
				{
					damageEvents.Clear();
					continue;
				}

				entity.Get<RendererComp>().WasHitTimer = 0.85f;

				HandleDamageEvents(in entity, damageEvents, ref playerHealth, ref score);
			}
		}

		private void HandleDamageEvents(in Entity entity, List<DamageEvent> damageEvents, ref HealthComp playerHealth, ref ScoreComp score)
		{
			float totalMultiplier = 1f;

			if (entity.Has<IsFlyingEnemy>())
				totalMultiplier *= _flyingDamageMultiplier;

			ref readonly var resistances = ref entity.Get<ResistancesComp>();
			bool isBoss = entity.Has<IsBossEnemy>();
			float totalDamage = 0f;

			for (int i = 0; i < damageEvents.Count; i++)
			{
				var damageEvent = damageEvents[i];

				float volatileMultiplier = resistances.GetPercent(damageEvent.DamageType);
				//Debug.Log($"Volatile: {volatileMultiplier}. DamageType: {damageEvent.DamageType}");

				if ((_farDamageIncrease || _closeDamageIncrease) && damageEvent.Source.Has<StartPositionComp>())
				{
					float projectileStartDistance = Vector2.Distance(damageEvent.Source.Get<StartPositionComp>().Value,
						entity.Get<PositionComp>().Value);

					volatileMultiplier *= Utilities.Utilities.Lerp3(_damageMinMultiplier, _damageMidMultiplier, _damageMaxMultiplier,
						projectileStartDistance / 300f);
					//Debug.Log($"Distance: {projectileStartDistance}, Multiplier: {_distanceMultiplier}");
				}

				ref var health = ref entity.Get<HealthComp>();
				if (_targetFullHealthDamageIncrease && health.Current == health.Max)
					volatileMultiplier *= _fullHealthDamageMultiplier;
				else if (_targetLowHealthDamageIncrease && health.Current <= health.Max * 0.25f)
					volatileMultiplier *= _lowHealthDamageMultiplier;

				//Player an be the source when applying fire damage, should the player have a crit comp?
				if (damageEvent.Source.Has<CritComp>())
				{
					ref var critComp = ref damageEvent.Source.Get<CritComp>();
					if (_alwaysCrit || critComp.Roll())
						volatileMultiplier *= critComp.Multiplier * _critMultiplier;
				}

				float damage = damageEvent.Damage * totalMultiplier * volatileMultiplier * _totalPlayerDamageMultiplier;
				//Debug.Log($"Damage: {damage}, Total Multiplier: {totalMultiplier}, " +
				//          $"Volatile Multiplier: {volatileMultiplier}, Player Multiplier: {_totalPlayerDamageMultiplier}");

				float originalHealth = health.Current;

				//Player an be the source when applying fire damage, should the player have a lifesteal comp?
				if (damageEvent.Source.Has<LifestealComp>())
				{
					float lifestealMultiplier = damageEvent.Source.Get<LifestealComp>().Multiplier;
					if (lifestealMultiplier != 0f)
					{
						playerHealth.Current +=
							Mathf.Min(damage * lifestealMultiplier * _lifestealMultiplier * _lifestealNegativeMultiplier,
								playerHealth.Max - playerHealth.Current);
					}
				}

				if (Random.value < _healOnAttackChance) //TODO PseudoRandomDistribution Roll
					damage = -damage; //TODO Do we care about over-healing?

				totalDamage += damage;
				health.Current -= damage;
				if (_bulletsNotLethal && health.Current <= 0f)
					health.Current = 1f;

				if (damage >= 0)
				{
					float damageDealt = originalHealth - health.Current;
					score.DamageDealt += damageDealt;
					if (damageEvent.DamageType == DamageType.Explosive)
						score.AoEDamage += damageDealt;

					score.SubmitLowestDamage(damageDealt);
				}

				//If kills and deals 2x more damage than original health. Add back 1 pierce. (unless max pierce is lower)
				if (health.Current <= 0 && originalHealth > 0 && damageEvent.Source.Has<PiercingComp>() && damage > originalHealth * 2f)
				{
					ref var pierceComp = ref damageEvent.Source.Get<PiercingComp>();
					if (_maxPierces > pierceComp.HitsLeft)
						pierceComp.HitsLeft++;
				}

				//TODO Lower hp % for bosses, instead of not working?
				//Last mechanic, culling blade
				if (_cullingProjectiles && !isBoss && health.Current > 0f && health.Current / health.Max < 0.15f)
					health.Current = 0f;
			}

			World.Publish(new DamageDealtEvent(entity, totalDamage));

			damageEvents.Clear();
		}
	}
}