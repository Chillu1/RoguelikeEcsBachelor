using System.Collections.Generic;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	public static class Wiki
	{
		private static readonly Dictionary<MechanicType, MechanicInfo> mechanics;

		static Wiki()
		{
			mechanics = new Dictionary<MechanicType, MechanicInfo>
			{
				{ MechanicType.None, new MechanicInfo("None", "Error") },

				{
					MechanicType.Damage,
					new MechanicInfo("Damage", "General damage that affects every damage calculation, projectile, AoE, circle")
				},
				{ MechanicType.ProjectileDamage, new MechanicInfo("Projectile Damage", "Damage dealt on projectile impact") },
				{ MechanicType.AoeRadius, new MechanicInfo("AoE Damage", "Damage dealt by AoE inflicted by AoE bullets, and circle") },
				{ MechanicType.Pierce, new MechanicInfo("Pierce", "Projectiles go through and damage multiple enemies, per pierce value") },
				{ MechanicType.Crit, new MechanicInfo("Crit", "Chance to deal multiplied damage by crit multiplier") },
				{
					MechanicType.Diminishing,
					new MechanicInfo("Diminishing",
						"Upgrade diminishes in value for each subsequent upgrade of same type.\nExample +5 value, 10%: +5, +4.5, +4.05")
				},

				{ MechanicType.AlwaysUpgrade, new MechanicInfo("Always Upgrade", "Special upgrades that have buffer states") },
				{
					MechanicType.MaxPierces,
					new MechanicInfo("Max Pierces",
						"How many times a projectile can max pierce through enemies. Overrides all other pierce mechanics. Can be reverted")
				},
				{
					MechanicType.ProjectilesHitPlayer,
					new MechanicInfo("Projectiles Hit Player",
						"Projectiles can hit player, and deal projectile impact damage, all modifiers work (lifesteal, crit). Projectiles can only hit player after they move 40 units from spawning")
				},
				{
					MechanicType.AoEDamagesPlayer,
					new MechanicInfo("AoE Damages Player", "AoE bullets also damage the player, if they're in the AoE radius")
				},
				{
					MechanicType.AlwaysCrit,
					new MechanicInfo("Always crit", "All damages always use crit multiplier. Other crit chance upgrades are not affected")
				},
				{
					MechanicType.DistanceBasedDamage,
					new MechanicInfo("Distance Based Damage",
						"Damage multiplier scales on distance. Stacks with other distance based upgrades")
				},
				{ MechanicType.HomingProjectiles, new MechanicInfo("Homing Projectiles", "Projectiles search once for a target to hit") },
				{ MechanicType.LowHealth, new MechanicInfo("Low Health", "When current health is 25% or lower than max health") },
				{ MechanicType.FullHealth, new MechanicInfo("Full Health", "When current health is equal to max health") },

				{
					MechanicType.CullingStrike,
					new MechanicInfo("Culling Strike", "Projectiles that hit an enemy with less than 15% health, die instantly")
				},
				{ MechanicType.DeathMark, new MechanicInfo("Death Mark", "Enemies marked for death die after X seconds") },
			};
		}

		public static MechanicInfo GetMechanicInfo(MechanicType type)
		{
			if (mechanics.TryGetValue(type, out var info))
				return info;

			Debug.LogError($"No info for mechanic type {type}");
			return mechanics[MechanicType.None];
		}
	}

	public sealed class MechanicInfo
	{
		public string Name { get; }
		public string Description { get; }

		public MechanicInfo(string name, string description)
		{
			Name = name;
			Description = description;
		}
	}

	public enum MechanicType
	{
		None,
		Damage,
		ProjectileDamage,
		AoeRadius,
		Pierce,
		Crit,
		Diminishing,

		AlwaysUpgrade,
		MaxPierces,
		ProjectilesHitPlayer,
		AoEDamagesPlayer,
		AlwaysCrit,
		DistanceBasedDamage,
		HomingProjectiles,
		LowHealth,
		FullHealth,

		CullingStrike,
		DeathMark,
		DeflectProjectiles,
	}
}