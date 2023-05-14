using System.Collections.Generic;
using DefaultEcs;
using DefaultEcs.System;
using RoguelikeEcs.Core.Utilities;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	[With(typeof(PlayerMouseClickComp))]
	public class ProjectileSpawnSystem : CustomEntitySetSystem
	{
		private readonly Sprite _projectileTexture;
		private readonly RuntimeAnimatorController _bulletAnimationController;
		private readonly GameObject _projectileParent;

		private readonly Quaternion _coneOffsetRight, _coneOffsetLeft;

		private int _piercesFlatAdd, _piercesMultiplier, _maxPierces;
		private bool _hasDistanceBasedDamage;
		private float _aoeRadiusMultiplier;
		private float _homingProjectilesChance;

		private const float DefaultBulletSpeed = 500f;
		private const float SniperMultiplier = 2f;
		private const float AoEMultiplier = 0.4f;


		public ProjectileSpawnSystem(World world) : base(world, SystemParameters.UsePlayer)
		{
			_projectileTexture = Resources.Load<Sprite>("Textures/Square");
			_bulletAnimationController = Resources.Load<RuntimeAnimatorController>("Animations/Projectile1");

			_projectileParent = new GameObject("Projectiles");

			_coneOffsetRight = Quaternion.Euler(0, 0, 30);
			_coneOffsetLeft = Quaternion.Euler(0, 0, -30);
		}

		protected override void PreUpdate(float state)
		{
			ref readonly var upgrades = ref World.Get<UpgradesComp>();
			_piercesFlatAdd = upgrades.PiercesFlatAdd;
			_piercesMultiplier = upgrades.PiercesMultiplier;
			_maxPierces = upgrades.MaxPierces;
			_hasDistanceBasedDamage = upgrades.HasFarDamage.IsOn() || upgrades.HasCloseDamage.IsOn();
			_homingProjectilesChance = upgrades.HomingChance;
			_aoeRadiusMultiplier = upgrades.AoERadiusMultiplier;

			ref var attackData = ref Player.Get<AttackDataComp>();
			attackData.AoEChance = upgrades.AoEChance;
			attackData.SniperChance = upgrades.SniperChance;
			attackData.LifeStealChance = upgrades.LifeStealChance;
			ref var homingData = ref Player.Get<HomingDataComp>();
			homingData.Chance = upgrades.HomingChance;
		}

		protected override void Update(float state, in Entity entity)
		{
			ref var playerAttack = ref entity.Get<AttackComp>();

			if (!playerAttack.CanAttack)
				return;

			Vector2 playerPosition = entity.Get<GameObjectComp>().Value.transform.position;
			var projectileDirection = (entity.Get<PlayerMouseClickComp>().Position - playerPosition).normalized;
			ref var attackData = ref entity.Get<AttackDataComp>();
			ref readonly var upgrades = ref World.Get<UpgradesComp>();

			ref var score = ref World.Get<ScoreComp>();

			var audio = Player.Get<AudioComp>();
			audio.Value.pitch = Random.Range(0.8f, 1.2f);
			audio.Value.PlayOneShot(audio.Clips[Random.Range(0, audio.Clips.Length)], (1f / audio.Value.volume) * 0.4f);

			score.ProjectilesFired++;
			SpawnProjectile(playerPosition, projectileDirection, ref attackData);
			if (upgrades.MultipleProjectilesCone.IsOn())
			{
				score.ProjectilesFired += 2;
				SpawnProjectile(playerPosition, _coneOffsetRight * projectileDirection, ref attackData);
				SpawnProjectile(playerPosition, _coneOffsetLeft * projectileDirection, ref attackData);
			}

			if (upgrades.MultipleProjectilesRapidFire.IsOn())
			{
				score.ProjectilesFired += 2;
				SpawnProjectile(playerPosition, projectileDirection, ref attackData);
				SpawnProjectile(playerPosition, projectileDirection, ref attackData);
			}

			if (upgrades.MultipleProjectilesSides.IsOn())
			{
				score.ProjectilesFired += 2;
				var rotation = Quaternion.FromToRotation(Vector2.up, projectileDirection);
				var right = rotation * Vector2.right;
				var left = rotation * Vector2.left;
				SpawnProjectile(playerPosition, right, ref attackData);
				SpawnProjectile(playerPosition, left, ref attackData);
			}

			//if (upgrades.MultipleProjectilesValley.IsOn())
			//{
			//	//Spawns projectiles on the sides of the main one, based on the rotation of the main projectile, moving in the same direction
			//}

			playerAttack.CanAttack = false;
		}

		internal Entity SpawnProjectile(Vector2 playerPosition, Vector2 projectileDirection, ref AttackDataComp attackData)
		{
			var wandPosition = Player.Get<WandComp>().TipPoint;

			ref readonly var upgrades = ref World.Get<UpgradesComp>();
			ref readonly var stats = ref Player.Get<StatsComp>();

			var projectile = World.CreateEntity();

			projectile.Set(new PositionComp { Value = wandPosition });
			projectile.Set(new StartPositionComp() { Value = wandPosition });

			var velocity = new VelocityComp
				{ Direction = projectileDirection, Speed = DefaultBulletSpeed + upgrades.ProjectileSpeedFlatAdd };

			var go = new GameObject(projectile.ToString());
			go.transform.position = wandPosition;
			//debugGo.AddComponent<BoxCollider2D>();
			go.transform.SetParent(_projectileParent.transform);
			var rb = go.AddComponent<Rigidbody2D>();
			rb.freezeRotation = true;
			rb.gravityScale = 0f;
			rb.interpolation = RigidbodyInterpolation2D.Interpolate;
			var damage = new DamageComp() { Value = 50f + stats.ExtraDamage };
			var crit = CritComp.Default;

			var piercing = new PiercingComp { EntitiesHit = new HashSet<Entity>(5), HitsLeft = 1 };
			var lifesteal = new LifestealComp();

			projectile.Set<IsProjectileComp>();
			projectile.Set<IsBulletProjectileComp>();

			if (_hasDistanceBasedDamage)
				projectile.Set(new StartPositionComp { Value = playerPosition });

			var timeOut = TimeOutComp.Default;
			if (upgrades.AlwaysHome.IsOn() || (!upgrades.AlwaysHome.IsOff() && Player.Get<HomingDataComp>().Roll()))
			{
				timeOut.TimeLeft *= 0.4f; //homing projectiles live shorter 
				projectile.Set<IsHomingProjectile>();
				projectile.Set<TargetComp>();
			}

			projectile.Set(timeOut);

			Color color = Color.white;
			Vector2 scale = new Vector2(1f, 1f) + Vector2.one * upgrades.ProjectileScaleMultiplier;
			bool isAoe = false;

			if (attackData.Roll(AttackType.Sniper))
			{
				velocity.Speed *= SniperMultiplier;
				color = Color.Lerp(color, Color.blue, 0.5f);
				scale = Vector2.Lerp(scale, new Vector2(0.5f, 0.5f), 0.5f);
				damage.Value *= SniperMultiplier;
				crit.Chance *= SniperMultiplier;
				crit.Multiplier += 0.5f;
				piercing.HitsLeft = 2;
			}

			if (upgrades.AlwaysAoE.IsOn() || (!upgrades.AlwaysAoE.IsOff() && attackData.Roll(AttackType.AoE)))
			{
				isAoe = true;
				projectile.Set<IsAoeProjectileComp>();
				projectile.Set(new AoEDamageComp { Value = damage.Value * AoEMultiplier * 0.5f });
				projectile.Set(new RadiusComp { Value = (30f + stats.ExtraAoERadius) * _aoeRadiusMultiplier });
				velocity.Speed *= AoEMultiplier;
				color = Color.Lerp(color, Color.magenta, 0.35f);
				scale = Vector2.Lerp(scale, new Vector2(1.7f, 1.7f), 0.5f);
				damage.Value *= AoEMultiplier;
				crit.Chance *= AoEMultiplier;
				crit.Multiplier -= 0.5f;
			}

			if (upgrades.AlwaysLifeSteal.IsOn() || (!upgrades.AlwaysLifeSteal.IsOff() && attackData.Roll(AttackType.LifeSteal)))
				lifesteal.Multiplier = 0.01f;

			if (upgrades.AlwaysFire.IsOn() || (!upgrades.AlwaysFire.IsOff() && attackData.Roll(EffectType.Burn)))
			{
				projectile.Set(new BurningDataComp { Duration = 5f, Damage = 10f });
				color = Color.Lerp(color, new Color(1f, 0.5f, 0f), 0.5f);
			}

			//if (attackData.Roll(EffectType.Poison))
			//{
			//	projectile.Set(new PoisonDataComp { Duration = 5f, Damage = 10f });
			//	color = Color.Lerp(color, Color.green, 0.5f);
			//	damage.Value *= 0.5f;
			//}

			if (upgrades.AlwaysCold.IsOn() || (!upgrades.AlwaysCold.IsOff() && attackData.Roll(EffectType.Cold)))
			{
				projectile.Set(new ColdDataComp { Duration = 5f, SlowMultiplier = 0.7f });
				color = Color.Lerp(color, Color.cyan, 0.5f);
			}

			//if (attackData.Roll(EffectType.Bleed))
			//{
			//	projectile.Set(new BleedingDataComp { Duration = 5f, Damage = 10f });
			//	color = Color.Lerp(color, new Color(0.85f, 0f, 0.1f), 0.5f);
			//	damage.Value *= 0.5f;
			//}

			if (!Core.IsTestMode)
			{
				var renderer = go.AddComponent<SpriteRenderer>();
				renderer.sprite = _projectileTexture;
				renderer.color = color;
			}

			projectile.Set(new ScaleComp { Value = scale, Magnitude = scale.GetMagnitude() });
			go.transform.localScale = scale;
			projectile.Set(damage);
			projectile.Set(crit);

			go.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(velocity.Direction.y, velocity.Direction.x) * Mathf.Rad2Deg - 90f);
			projectile.Set(velocity);
			piercing.HitsLeft += _piercesFlatAdd;
			piercing.HitsLeft *= _piercesMultiplier;
			if (isAoe)
				piercing.HitsLeft = Mathf.CeilToInt(piercing.HitsLeft * 0.5f);
			piercing.HitsLeft = Mathf.Min(piercing.HitsLeft, _maxPierces);
			projectile.Set(piercing);
			projectile.Set(lifesteal);
			projectile.Set(new GameObjectComp() { Value = go });
			projectile.Set(new RigidbodyComp() { Value = rb });
			var animation = new AnimationComp() { Value = go.AddComponent<Animator>() };
			animation.Value.runtimeAnimatorController = _bulletAnimationController;
			projectile.Set(animation);

			return projectile;
		}
	}
}