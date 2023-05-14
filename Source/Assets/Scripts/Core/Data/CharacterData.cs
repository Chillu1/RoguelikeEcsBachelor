using System.Collections.Generic;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	public sealed class CharacterData
	{
		private readonly Dictionary<CharacterType, CharacterInfo> _characters;

		public CharacterData()
		{
			_characters = new Dictionary<CharacterType, CharacterInfo>();
			SetupCharacters();
		}

		public IReadOnlyCollection<CharacterInfo> GetCharactersInfo() => _characters.Values;

		public CharacterInfo GetCharacterInfo(CharacterType type)
		{
			if (_characters.TryGetValue(type, out var characterInfo))
				return characterInfo;

			Debug.LogError($"CharacterInfo for {type} not found");
			return _characters[CharacterType.Default];
		}

		private void SetupCharacters()
		{
			CharacterInfo Add(CharacterType type)
			{
				var characterInfo = new CharacterInfo(type);
				_characters.Add(type, characterInfo);
				return characterInfo;
			}

			Add(CharacterType.Default)
				.SetName("Basic Wizard");

			Add(CharacterType.AoE) //TODO Projectile speed multiplier 0.6f
				.SetName("AoE Knight Wizard")
				.SetDescription("50% bigger AoE radius, 50% more AoE chance, +10 AoE radius" +
				                ", 50% slower attack speed, 10% less sniper chance")
				.Unlockable((data, world, player, score) => data.TotalAoEDamage >= 2_000, "Deal 2k AoE damage") //TODO 20k balance
				.Multipliers((StatType.AttackSpeed, 0.5f))
				.Upgrade((ref UpgradesComp upgrades) =>
				{
					upgrades.AoERadiusMultiplier += 0.5f;
					upgrades.AoEChance += 0.5f;
					upgrades.SniperChance -= UpgradesComp.DefaultSniperChance;
				})
				.Stats((ref StatsComp stats) => stats.ExtraAoERadius += 10f);

			//TODO Better pacifist bonus, lower damage, higher health, flat heal on attack, stronger knockback, less enemies spawning
			Add(CharacterType.Pacifist)
				.SetName("Pacifist Wizard")
				.SetDescription(
					"Better pacifist bonus, 50% higher health, flat heal on attack, stronger knockback, less enemies spawning, 80% less damage")
				.UnlockableWaveEnd((data, world, player, score) => score.DamageDealt <= 0, "Deal 0 damage in a wave");

			//TODO Flat evasion, higher evasion multiplier, higher move speed, lower resistances multiplier
			Add(CharacterType.Evasive)
				.SetName("Evasive Wizard")
				.SetDescription("")
				.UnlockableWaveEnd((data, world, player, score) => score.DamageTaken <= 0, "Take 0 damage in a wave");

			//TODO Finish up/downsides
			Add(CharacterType.Survivor)
				.SetName("Survivor Wizard")
				.SetDescription("")
				.UnlockableWaveEnd(
					(data, world, player, score) => player.Get<PreviousHealthComp>().Value / player.Get<HealthComp>().Max < 0.05f,
					"Survive a wave with 5% hp or less");

			//TODO Always Home or 100% chance, or mix of both?
			Add(CharacterType.Homer)
				.SetName("Homer Wizard")
				.SetDescription("+100% homing projectile chance, 0.4X damage")
				.UnlockableWaveEnd((data, world, player, score) => score.EnemiesHitWithProjectiles <= 0, "Miss all projectiles in a wave")
				.Multipliers((StatType.Damage, 0.4f))
				.Upgrade((ref UpgradesComp upgrades) => upgrades.HomingChance += 1f);

			//TODO more resistances, less evasion
			Add(CharacterType.Slow)
				.SetName("Slow Wizard")
				.SetDescription("50% more damage, 200% more health, 50% more resistances, 30% less move speed, 50% less evasion")
				.UnlockableWaveEnd((data, world, player, score) => score.DistanceTraveled <= 0, "Don't move in a wave")
				.Multipliers((StatType.Damage, 1.5f), (StatType.Health, 3f), (StatType.MoveSpeed, 0.7f));

			Add(CharacterType.Speedy)
				.SetName("Speedy Wizard")
				.SetDescription("1.5X move speed, 2X attack speed, 0.5X damage, 0.5X health")
				.Unlockable((data, world, player, score) => data.DistanceTraveled >= 60_000, "Move 60 000 units")
				.Multipliers((StatType.MoveSpeed, 1.5f), (StatType.AttackSpeed, 2f), (StatType.Damage, 0.5f), (StatType.Health, 0.5f));

			Add(CharacterType.Sniper)
				.SetName("Sniper Wizard")
				.SetDescription("+100% sniper chance, 2X damage, +100 projectile speed, 0.5X health, 0.5X attack speed")
				.UnlockableWaveEnd((data, world, player, score) => score.MissedProjectiles == 0, "Miss 0 projectiles in a wave")
				.Multipliers((StatType.Damage, 2f), (StatType.Health, 0.5f), (StatType.AttackSpeed, 0.5f))
				.Upgrade((ref UpgradesComp upgrades) =>
				{
					upgrades.SniperChance += 1f;
					upgrades.ProjectileSpeedFlatAdd += 100f;
				});

			Add(CharacterType.GlassCannon)
				.SetName("Glass Cannon Wizard")
				.SetDescription("2X damage, 0.25X health")
				.Unlockable((data, world, player, score) =>
				{
					ref readonly var health = ref player.Get<HealthComp>();
					return health.IsDead && player.Get<PreviousHealthComp>().Value >= health.Max;
				}, "Get one shot from full health")
				.Multipliers((StatType.Damage, 2f), (StatType.Health, 0.25f));

			Add(CharacterType.SoyMilk)
				.SetName("Soy Milk Wizard")
				.SetDescription("5X attack speed, 0.1X damage")
				.Unlockable((data, world, player, score) => score.LowestDamage <= 4, "Deal 4 or less damage")
				.Multipliers((StatType.AttackSpeed, 5f), (StatType.Damage, 0.1f))
				.Upgrade((ref UpgradesComp upgrades) => upgrades.ProjectileScaleMultiplier *= 0.5f);
		}
	}
}