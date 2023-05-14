using System.Collections.Generic;
using System.Linq;
using DefaultEcs;
using RoguelikeEcs.Core.Utilities;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	public sealed class UpgradesData
	{
		private readonly Dictionary<UpgradeType, Upgrade> _upgrades;

		public UpgradesData()
		{
			_upgrades = new Dictionary<UpgradeType, Upgrade>();
			SetupBasicUpgrades();
			SetupUpgrades();

			foreach (var upgrade in _upgrades.Values)
				upgrade.Validate();
		}

		public IReadOnlyDictionary<UpgradeType, Upgrade> GetUpgrades() => _upgrades.ToDictionary(x => x.Key, x => x.Value.ShallowClone());

		private void SetupBasicUpgrades()
		{
			Upgrade Add(UpgradeType upgradeType)
			{
				var upgrade = new Upgrade(upgradeType);
				_upgrades.Add(upgradeType, upgrade);
				return upgrade;
			}

			Add(UpgradeType.AttackSpeed)
				.SetName("Attack Speed +10 (diminishing 5%)")
				.SetWeight(300)
				.SetIsReappliable()
				.SetIsDiminishing()
				.SetMechanics(MechanicType.Diminishing)
				.SetAction((float upgradeMulti, ref StatsComp stats, ref UpgradesComp _, ref MultiplierComp _, ref EnemyUpgradesComp _) =>
				{
					stats.ExtraAttackSpeed += 10f * upgradeMulti;
				});

			Add(UpgradeType.Damage)
				.SetName("Damage +10 (diminishing 5%)")
				.SetWeight(300)
				.SetIsReappliable()
				.SetIsDiminishing()
				.SetMechanics(MechanicType.Damage, MechanicType.Diminishing)
				.SetAction((float upgradeMulti, ref StatsComp stats, ref UpgradesComp _, ref MultiplierComp _, ref EnemyUpgradesComp _) =>
				{
					stats.ExtraDamage += 10 * upgradeMulti;
				});

			Add(UpgradeType.Health)
				.SetName("Health +40")
				.SetWeight(300)
				.SetIsReappliable()
				.SetAction((float _, ref StatsComp stats, ref UpgradesComp _, ref MultiplierComp multiplier,
					ref EnemyUpgradesComp _) =>
				{
					stats.ExtraHealth += 40;
					//TODO Temporary health update fix
					multiplier.MultipliersList.Add(new MultiplierData { StatType = StatType.Health, Multiplier = 1f });
				});

			Add(UpgradeType.MoveSpeed)
				.SetName("Move Speed +15 (default 200, diminishing 15%)")
				.SetWeight(300)
				.SetIsReappliable()
				.SetIsDiminishing(0.15f)
				.SetMechanics(MechanicType.Diminishing)
				.SetAction((float upgradeMulti, ref StatsComp stats, ref UpgradesComp _, ref MultiplierComp _, ref EnemyUpgradesComp _) =>
				{
					stats.ExtraMoveSpeed += 15 * upgradeMulti;
				});

			Add(UpgradeType.AoERadius)
				.SetName("AoE Radius +5 (diminishing 5%)")
				.SetWeight(150)
				.SetRarity(UpgradeRarity.Uncommon)
				.SetIsReappliable()
				.SetIsDiminishing()
				.SetMechanics(MechanicType.AoeRadius, MechanicType.Diminishing)
				.SetAction((float upgradeMulti, ref StatsComp stats, ref UpgradesComp _, ref MultiplierComp _, ref EnemyUpgradesComp _) =>
				{
					stats.ExtraAoERadius += 5 * upgradeMulti;
				});

			Add(UpgradeType.BulletDamage)
				.SetName("Bullet Damage +15 (diminishing 5%)")
				.SetWeight(150)
				.SetRarity(UpgradeRarity.Uncommon)
				.SetIsReappliable()
				.SetIsDiminishing()
				.SetMechanics(MechanicType.ProjectileDamage, MechanicType.Diminishing)
				.SetAction((float upgradeMulti, ref StatsComp stats, ref UpgradesComp _, ref MultiplierComp _, ref EnemyUpgradesComp _) =>
				{
					stats.ExtraBulletDamage += 15f * upgradeMulti;
				});

			Add(UpgradeType.HomingProjectiles)
				.SetName("Homing Projectiles +10%")
				.SetWeight(50)
				.SetRarity(UpgradeRarity.Uncommon)
				.SetIsReappliable()
				.SetMechanics(MechanicType.HomingProjectiles)
				.SetAction((float _, ref StatsComp _, ref UpgradesComp upgrades, ref MultiplierComp _, ref EnemyUpgradesComp _) =>
				{
					upgrades.HomingChance += 0.1f;
				});

			Add(UpgradeType.ProjectileSpeed)
				.SetName("Projectile Speed +50 (default 500, diminishing 5%)")
				.SetWeight(50)
				.SetRarity(UpgradeRarity.Uncommon)
				.SetIsReappliable()
				.SetIsDiminishing()
				.SetMechanics(MechanicType.Diminishing)
				.SetAction(
					(float upgradeMulti, ref StatsComp _, ref UpgradesComp upgrades, ref MultiplierComp _, ref EnemyUpgradesComp _) =>
					{
						upgrades.ProjectileSpeedFlatAdd += 50f * upgradeMulti;
					});

			Add(UpgradeType.AoEChance)
				.SetName("AoE Chance +5% (diminishing 10%)")
				.SetWeight(100)
				.SetRarity(UpgradeRarity.Uncommon)
				.SetIsReappliable()
				.SetIsDiminishing(0.1f)
				.SetMechanics(MechanicType.Diminishing)
				.SetAction(
					(float upgradeMulti, ref StatsComp _, ref UpgradesComp upgrades, ref MultiplierComp _, ref EnemyUpgradesComp _) =>
					{
						upgrades.AoEChance += 0.05f * upgradeMulti;
					});

			Add(UpgradeType.SniperChance)
				.SetName("Sniper Chance +5% (diminishing 10%)")
				.SetWeight(100)
				.SetRarity(UpgradeRarity.Uncommon)
				.SetIsReappliable()
				.SetIsDiminishing(0.1f)
				.SetMechanics(MechanicType.Diminishing)
				.SetAction(
					(float upgradeMulti, ref StatsComp _, ref UpgradesComp upgrades, ref MultiplierComp _, ref EnemyUpgradesComp _) =>
					{
						upgrades.SniperChance += 0.05f * upgradeMulti;
					});

			//Add(UpgradeType.Evasion)
		}

		private void SetupUpgrades()
		{
			Upgrade Add(UpgradeType upgradeType)
			{
				var upgrade = new Upgrade(upgradeType);
				_upgrades.Add(upgradeType, upgrade);
				return upgrade;
			}

			Add(UpgradeType.AlwaysAoENeverPierce)
				.SetName("Always AoE, Max 1 hit on projectiles (0 pierces)")
				.SetWeight(50)
				.SetUnlockable((gameData, world, player, score) => gameData.TotalKills >= 2)
				.SetRarity(UpgradeRarity.Epic)
				.SetMechanics(MechanicType.Pierce, MechanicType.MaxPierces, MechanicType.AlwaysUpgrade)
				.SetAction((float _, ref StatsComp _, ref UpgradesComp upgrades, ref MultiplierComp _, ref EnemyUpgradesComp _) =>
				{
					upgrades.MaxPierces = 1;
					upgrades.AlwaysAoE.Enable();
				});

			Add(UpgradeType.Lifesteal2XZeroRegen)
				.SetName("2X Lifesteal, 0 Regen")
				.SetWeight(50)
				.SetRarity(UpgradeRarity.Epic)
				.SetAction((float _, ref StatsComp _, ref UpgradesComp upgrades, ref MultiplierComp _, ref EnemyUpgradesComp _) =>
				{
					upgrades.RegenMultiplier = 0; //TODO UpgradeState instead of multiplier
					upgrades.LifestealMultiplier *= 2;
				});

			Add(UpgradeType.Pierce10FlatDamage0_5Multiplier)
				.SetName("Pierce +10, Damage Multiplier 0.5X")
				.SetWeight(50)
				.SetRarity(UpgradeRarity.Epic)
				.SetMechanics(MechanicType.Pierce)
				.SetAction((float _, ref StatsComp _, ref UpgradesComp upgrades, ref MultiplierComp multiplier, ref EnemyUpgradesComp _) =>
				{
					upgrades.PiercesFlatAdd += 10;
					multiplier.MultipliersList.Add(new MultiplierData() { StatType = StatType.Damage, Multiplier = 0.5f });
				});

			Add(UpgradeType.Regen5FlatLifesteal0Multiplier)
				.SetName("Health regen +5, Lifesteal Multiplier 0")
				.SetWeight(50)
				.SetRarity(UpgradeRarity.Epic)
				.SetAction((float _, ref StatsComp _, ref UpgradesComp upgrades, ref MultiplierComp _, ref EnemyUpgradesComp _) =>
				{
					upgrades.RegenFlatAdd += 0.5f;
					upgrades.LifestealMultiplier = 0; //TODO UpgradeState instead of multiplier
				});

			Add(UpgradeType.Bullets4XDamageSelfHit) //Unlocked by player killing themselves? (by some mean, aoe?)
				.SetName("Bullet damage 4X. Bullets can hit player")
				.SetWeight(50)
				.SetRarity(UpgradeRarity.Epic)
				.SetMechanics(MechanicType.ProjectilesHitPlayer)
				.SetAction((float _, ref StatsComp _, ref UpgradesComp upgrades, ref MultiplierComp _, ref EnemyUpgradesComp _) =>
				{
					upgrades.BulletDamageMultiplier *= 4;
					upgrades.CanHitPlayer.Enable();
				});

			//Add(UpgradeType.TODOAoEAlsoDamagesPlayer)
			//	.SetName(". AoE also damages player")
			//	.SetWeight(1) //TODO
			//	.SetRarity(UpgradeRarity.Epic)
			//	.SetMechanics(MechanicType.AoEDamagesPlayer)
			//	.SetAction((float _, ref StatsComp _, ref UpgradesComp upgrades, ref MultiplierComp _, ref EnemyUpgradesComp _) =>
			//	{
			//		//TODO Upside
			//		upgrades.CanHitPlayerWithAoe.Enable();
			//	});

			Add(UpgradeType.PlayerAlwaysCritEnemiesAlwaysCrit) //Die to an enemy crit, in one shot?
				.SetName("Player 100% chance to crit. Enemies 100% chance to crit. Increase both crit multi 1.5X")
				.SetWeight(50)
				.SetRarity(UpgradeRarity.Epic)
				.SetMechanics(MechanicType.Crit, MechanicType.AlwaysUpgrade, MechanicType.AlwaysCrit)
				.SetAction((float _, ref StatsComp _, ref UpgradesComp upgrades, ref MultiplierComp _,
					ref EnemyUpgradesComp enemyUpgrades) =>
				{
					upgrades.AlwaysCrit.Enable();
					upgrades.CritMultiplier *= 1.5f;
					enemyUpgrades.AlwaysCrit.Enable();
					enemyUpgrades.CritMultiplier *= 1.5f;
				});

			Add(UpgradeType.AlwaysAoEAttackSpeed0_5X)
				.SetName("All attacks are AoE, attack speed multiplier 0.5X")
				.SetWeight(50)
				.SetRarity(UpgradeRarity.Epic)
				.SetMechanics(MechanicType.AlwaysUpgrade)
				.SetAction((float _, ref StatsComp _, ref UpgradesComp upgrades, ref MultiplierComp multiplier,
					ref EnemyUpgradesComp _) =>
				{
					upgrades.AlwaysAoE.Enable();
					multiplier.MultipliersList.Add(new MultiplierData() { StatType = StatType.AttackSpeed, Multiplier = 0.5f });
				});

			Add(UpgradeType.RegenLifesteal2XHealOnAttack)
				.SetName("Regen and lifesteal increased 2X. 20% chance to heal enemy on attack")
				.SetWeight(50)
				.SetRarity(UpgradeRarity.Epic)
				.SetAction((float _, ref StatsComp _, ref UpgradesComp upgrades, ref MultiplierComp _, ref EnemyUpgradesComp _) =>
				{
					upgrades.RegenMultiplier *= 2;
					upgrades.LifestealMultiplier *= 2;
					upgrades.HealOnAttack.Enable();
					upgrades.HealOnAttackChance = 0.2f;
				});

			Add(UpgradeType.FarDamage)
				.SetName("Distance based damage multiplier. Far: 3X. Close: 0.5X")
				.SetWeight(50)
				.SetRarity(UpgradeRarity.Epic)
				.SetMechanics(MechanicType.DistanceBasedDamage)
				.SetAction((float _, ref StatsComp _, ref UpgradesComp upgrades, ref MultiplierComp _, ref EnemyUpgradesComp _) =>
				{
					upgrades.HasFarDamage.Enable();
				});

			Add(UpgradeType.CloseDamage)
				.SetName("Distance based damage multiplier. Far: 0.5X. Close: 3X")
				.SetWeight(50)
				.SetRarity(UpgradeRarity.Epic)
				.SetMechanics(MechanicType.DistanceBasedDamage)
				.SetAction((float _, ref StatsComp _, ref UpgradesComp upgrades, ref MultiplierComp _, ref EnemyUpgradesComp _) =>
				{
					upgrades.HasCloseDamage.Enable();
				});

			Add(UpgradeType.CullingProjectiles)
				.SetName("Culling Projectiles")
				.SetWeight(50)
				.SetRarity(UpgradeRarity.Epic)
				.SetMechanics(MechanicType.CullingStrike)
				.SetAction((float _, ref StatsComp _, ref UpgradesComp upgrades, ref MultiplierComp _, ref EnemyUpgradesComp _) =>
				{
					upgrades.HasCullingProjectiles.Enable();
				});

			Add(UpgradeType.FullHealthDamage)
				.SetName("Deal 2X damage to full health enemies")
				.SetWeight(50)
				.SetRarity(UpgradeRarity.Epic)
				.SetMechanics(MechanicType.FullHealth)
				.SetAction((float _, ref StatsComp _, ref UpgradesComp upgrades, ref MultiplierComp _, ref EnemyUpgradesComp _) =>
				{
					upgrades.HasFullHealthDamage.Enable();
				});

			Add(UpgradeType.LowHealthDamage)
				.SetName("Deal 2X damage to low health enemies")
				.SetWeight(50)
				.SetRarity(UpgradeRarity.Epic)
				.SetMechanics(MechanicType.LowHealth)
				.SetAction((float _, ref StatsComp _, ref UpgradesComp upgrades, ref MultiplierComp _, ref EnemyUpgradesComp _) =>
				{
					upgrades.HasLowHealthDamage.Enable();
				});

			Add(UpgradeType.PlayerFullHealthDamage)
				.SetName("Deal 2X damage on full health")
				.SetWeight(50)
				.SetRarity(UpgradeRarity.Epic)
				.SetMechanics(MechanicType.FullHealth)
				.SetAction((float _, ref StatsComp _, ref UpgradesComp upgrades, ref MultiplierComp _, ref EnemyUpgradesComp _) =>
				{
					upgrades.HasPlayerFullHealthDamage.Enable();
				});

			Add(UpgradeType.PlayerLowHealthDamage)
				.SetName("Deal 2X damage on low health")
				.SetWeight(50)
				.SetRarity(UpgradeRarity.Epic)
				.SetMechanics(MechanicType.LowHealth)
				.SetAction((float _, ref StatsComp _, ref UpgradesComp upgrades, ref MultiplierComp _, ref EnemyUpgradesComp _) =>
				{
					upgrades.HasPlayerLowHealthDamage.Enable();
				});

			Add(UpgradeType.AlwaysFireNeverCold)
				.SetName("100% chance to fire, 0% chance to cold")
				.SetWeight(50)
				.SetRarity(UpgradeRarity.Epic)
				.SetMechanics(MechanicType.AlwaysUpgrade)
				.SetAction((float _, ref StatsComp _, ref UpgradesComp upgrades, ref MultiplierComp _, ref EnemyUpgradesComp _) =>
				{
					upgrades.AlwaysFire.Enable();
					upgrades.AlwaysCold.Disable();
				});

			Add(UpgradeType.AlwaysColdNeverFire)
				.SetName("100% chance to cold, 0% chance to fire")
				.SetWeight(50)
				.SetRarity(UpgradeRarity.Epic)
				.SetMechanics(MechanicType.AlwaysUpgrade)
				.SetAction((float _, ref StatsComp _, ref UpgradesComp upgrades, ref MultiplierComp _, ref EnemyUpgradesComp _) =>
				{
					upgrades.AlwaysCold.Enable();
					upgrades.AlwaysFire.Disable();
				});

			Add(UpgradeType.Damage2XNeverFireNeverCold)
				.SetName("Damage X2, 0% chance to cold, 0% chance to fire")
				.SetWeight(50)
				.SetRarity(UpgradeRarity.Epic)
				.SetMechanics(MechanicType.AlwaysUpgrade)
				.SetAction((float _, ref StatsComp _, ref UpgradesComp upgrades, ref MultiplierComp multiplier, ref EnemyUpgradesComp _) =>
				{
					multiplier.MultipliersList.Add(new MultiplierData() { StatType = StatType.Damage, Multiplier = 2 });
					upgrades.AlwaysFire.Disable();
					upgrades.AlwaysCold.Disable();
				});

			Add(UpgradeType.AlwaysHomeNeverPierce)
				.SetName("100% chance for homing projectile, Never pierce")
				.SetWeight(50)
				.SetRarity(UpgradeRarity.Legendary)
				.SetMechanics(MechanicType.Pierce, MechanicType.AlwaysUpgrade)
				.SetAction((float _, ref StatsComp _, ref UpgradesComp upgrades, ref MultiplierComp _, ref EnemyUpgradesComp _) =>
				{
					upgrades.AlwaysHome.Enable();
					upgrades.MaxPierces = 1;
				});

			Add(UpgradeType.EffectsMoreStacks)
				.SetName("Effects can be stacked more times")
				.SetWeight(50)
				.SetRarity(UpgradeRarity.Epic)
				.SetMechanics(MechanicType.AlwaysUpgrade)
				.SetAction((float _, ref StatsComp _, ref UpgradesComp playerUpgrades, ref MultiplierComp _, ref EnemyUpgradesComp _) =>
				{
					playerUpgrades.EffectsMoreStacks.Enable();
				});

			Add(UpgradeType.ProjectileSpeedNeverAoE)
				.SetName("Projectile speed, Never AoE")
				.SetWeight(50)
				.SetRarity(UpgradeRarity.Uncommon)
				.SetMechanics(MechanicType.AlwaysUpgrade)
				.SetAction((float _, ref StatsComp _, ref UpgradesComp playerUpgrades, ref MultiplierComp _, ref EnemyUpgradesComp _) =>
				{
					playerUpgrades.ProjectileSpeedFlatAdd += 200;
					playerUpgrades.AlwaysAoE.Disable();
				});

			Add(UpgradeType.BigProjectiles)
				.SetName("Bigger projectile, higher projectile dmg, lower projectile speed, lower attack speed")
				.SetWeight(50)
				.SetUnlockable((data, world, player, score) => data.TotalBossKills >= 10)
				.SetRarity(UpgradeRarity.Epic)
				.SetMechanics(MechanicType.ProjectileDamage)
				.SetAction((float _, ref StatsComp _, ref UpgradesComp playerUpgrades, ref MultiplierComp multiplier,
					ref EnemyUpgradesComp _) =>
				{
					playerUpgrades.ProjectileScaleMultiplier *= 1.5f;
					multiplier.MultipliersList.Add(new MultiplierData() { StatType = StatType.Damage, Multiplier = 2f });
					playerUpgrades.ProjectileSpeedFlatAdd -= 200;
					multiplier.MultipliersList.Add(new MultiplierData() { StatType = StatType.AttackSpeed, Multiplier = 0.5f });
				});

			Add(UpgradeType.SmallProjectiles)
				.SetName("Smaller projectile, lower projectile dmg, higher projectile speed, higher attack speed")
				.SetWeight(50)
				.SetUnlockable((data, world, player, score) => player.Get<RigidbodyComp>().Value.velocity.GetMagnitude() >= 260)
				.SetRarity(UpgradeRarity.Epic)
				.SetMechanics(MechanicType.ProjectileDamage)
				.SetAction((float _, ref StatsComp _, ref UpgradesComp playerUpgrades, ref MultiplierComp multiplier,
					ref EnemyUpgradesComp _) =>
				{
					playerUpgrades.ProjectileScaleMultiplier *= 0.5f;
					multiplier.MultipliersList.Add(new MultiplierData() { StatType = StatType.Damage, Multiplier = 0.5f });
					playerUpgrades.ProjectileSpeedFlatAdd += 200;
					multiplier.MultipliersList.Add(new MultiplierData() { StatType = StatType.AttackSpeed, Multiplier = 2f });
				});

			Add(UpgradeType.MultipleProjectilesCone)
				.SetName("+2 projectiles, in a cone. Attack speed 0.33X")
				.SetWeight(50)
				.SetRarity(UpgradeRarity.Epic)
				.SetAction((float _, ref StatsComp _, ref UpgradesComp playerUpgrades, ref MultiplierComp multiplier,
					ref EnemyUpgradesComp _) =>
				{
					playerUpgrades.MultipleProjectilesCone.Enable();
					multiplier.MultipliersList.Add(new MultiplierData() { StatType = StatType.AttackSpeed, Multiplier = 0.33f });
				});

			Add(UpgradeType.MultipleProjectilesRapidFire)
				.SetName("+2 projectiles, rapid fire. Damage 0.25X")
				.SetWeight(50)
				.SetUnlockable((data, world, player, score) => data.ProjectilesFired >= 1000)
				.SetRarity(UpgradeRarity.Epic)
				.SetAction((float _, ref StatsComp _, ref UpgradesComp playerUpgrades, ref MultiplierComp multiplier,
					ref EnemyUpgradesComp _) =>
				{
					playerUpgrades.MultipleProjectilesRapidFire.Enable();
					multiplier.MultipliersList.Add(new MultiplierData() { StatType = StatType.Damage, Multiplier = 0.25f });
				});

			Add(UpgradeType.MultipleProjectilesSides)
				.SetName("+2 projectiles, left right sides. Damage 0.5X")
				.SetWeight(50)
				.SetRarity(UpgradeRarity.Epic)
				.SetAction((float _, ref StatsComp _, ref UpgradesComp playerUpgrades, ref MultiplierComp multiplier,
					ref EnemyUpgradesComp _) =>
				{
					playerUpgrades.MultipleProjectilesSides.Enable();
					multiplier.MultipliersList.Add(new MultiplierData() { StatType = StatType.Damage, Multiplier = 0.5f });
				});

			Add(UpgradeType.AttackSpeed5X_Damage0_1X)
				.SetName("Attack speed 5X, Damage 0.1X")
				.SetWeight(50)
				.SetRarity(UpgradeRarity.Epic)
				.SetMechanics(MechanicType.Damage)
				.SetAction((float _, ref StatsComp _, ref UpgradesComp _, ref MultiplierComp multiplier, ref EnemyUpgradesComp _) =>
				{
					multiplier.MultipliersList.Add(new MultiplierData() { StatType = StatType.AttackSpeed, Multiplier = 5f });
					multiplier.MultipliersList.Add(new MultiplierData() { StatType = StatType.Damage, Multiplier = 0.1f });
				});

			Add(UpgradeType.AoEChance50_SniperChanceMinus50)
				.SetName("AoE chance 50%, Sniper chance -50%")
				.SetWeight(50)
				.SetRarity(UpgradeRarity.Epic)
				.SetAction((float _, ref StatsComp _, ref UpgradesComp playerUpgrades, ref MultiplierComp _, ref EnemyUpgradesComp _) =>
				{
					playerUpgrades.AoEChance += 0.5f;
					playerUpgrades.SniperChance -= 0.5f;
				});

			Add(UpgradeType.SniperChance50_AoEChanceMinus50)
				.SetName("Sniper chance 50%, AoE chance -50%")
				.SetWeight(50)
				.SetRarity(UpgradeRarity.Epic)
				.SetAction((float _, ref StatsComp _, ref UpgradesComp playerUpgrades, ref MultiplierComp _, ref EnemyUpgradesComp _) =>
				{
					playerUpgrades.SniperChance += 0.5f;
					playerUpgrades.AoEChance -= 0.5f;
				});
		}
	}
}