using System.Collections.Generic;

namespace RoguelikeEcs.Core
{
	public struct UpgradesComp //TODO Rename
	{
		public Dictionary<UpgradeType, int> UpgradesData;

		public bool AutomaticAim;

		public UpgradeState HasFarDamage;
		public UpgradeState HasCloseDamage;
		public UpgradeState HasFullHealthDamage;
		public UpgradeState HasLowHealthDamage;
		public UpgradeState HasPlayerFullHealthDamage;
		public UpgradeState HasPlayerLowHealthDamage;
		public UpgradeState HasBulletsNotLethal;
		public UpgradeState CanHitPlayer, CanHitPlayerWithAoe;
		public UpgradeState AlwaysCrit;
		public UpgradeState AlwaysAoE;
		public UpgradeState AlwaysLifeSteal;
		public UpgradeState HealOnAttack;
		public UpgradeState HasCullingProjectiles;
		public UpgradeState AlwaysFire;
		public UpgradeState AlwaysCold;
		public UpgradeState AlwaysHome;
		public UpgradeState EffectsMoreStacks;

		public UpgradeState MultipleProjectilesCone;
		public UpgradeState MultipleProjectilesRapidFire;
		public UpgradeState MultipleProjectilesSides;

		public bool HealingNegative, LifestealNegative;

		public float ProjectileSpeedFlatAdd, ProjectileScaleMultiplier;

		public int PiercesFlatAdd;
		public int PiercesMultiplier;
		public int MaxPierces; //TODO To UpgradeState

		public float BulletDamageMultiplier;
		public float HomingChance;

		public float AoERadiusMultiplier;

		public float FlyingDamageMultiplier;

		public float RegenFlatAdd;
		public float RegenMultiplier;
		public float LifestealMultiplier;
		public float CritMultiplier;

		public float AoEChance;
		public float SniperChance;
		public float LifeStealChance;

		public float HealOnAttackChance;

		//public float PlayerAttackCooldown;

		// Player Curse
		public bool HasDeflectionCurse;
		public bool HasDamageBoostCurse;

		// Circle effect
		public float CircleEffectDurationMultiplier;
		public bool HasCircleDamage;
		public float CircleDamageModifier;

		public bool HasCircleBurn;
		public float CircleBurnDamageMultiplier;

		public bool HasCircleCold;
		public float CircleColdSlowMultiplier;

		public bool HasCircleHealPlayer;

		public bool HasCirclePoison;
		public float CirclePoisonDamageMultiplier;

		public UpgradeState HasDeathMark;

		public const float DefaultSniperChance = 0.15f;
		public const float DefaultAoEChance = 0.15f;

		public static UpgradesComp Default => new UpgradesComp
		{
			UpgradesData = new Dictionary<UpgradeType, int>(),

			PiercesMultiplier = 1,
			MaxPierces = int.MaxValue,

			BulletDamageMultiplier = 1f,

			AoERadiusMultiplier = 1f,

			FlyingDamageMultiplier = 1f,
			RegenMultiplier = 1f,
			LifestealMultiplier = 1f,
			CritMultiplier = 1f,

			HomingChance = 0f,
			SniperChance = DefaultSniperChance,
			AoEChance = DefaultAoEChance,
			LifeStealChance = 0f,
			//PlayerAttackCooldown = 0.5f,

			HasDeflectionCurse = true,

			CircleEffectDurationMultiplier = 1f,
			HasCircleDamage = true,
			CircleDamageModifier = 1f,
			HasCircleBurn = false,
			CircleBurnDamageMultiplier = 1f,
			HasCircleCold = true,
			CircleColdSlowMultiplier = 0.6f,
			HasCircleHealPlayer = false,
			HasCirclePoison = false,
			CirclePoisonDamageMultiplier = 1f,
		};
	}
}