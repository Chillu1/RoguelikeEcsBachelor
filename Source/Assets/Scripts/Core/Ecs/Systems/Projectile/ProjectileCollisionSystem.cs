using System;
using System.Collections.Generic;
using System.Linq;
using DefaultEcs;
using DefaultEcs.Internal;
using DefaultEcs.System;
using RoguelikeEcs.Core.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RoguelikeEcs.Core
{
	[With(typeof(IsProjectileComp))]
	public sealed class ProjectileCollisionSystem : CustomEntitySetSystem
	{
		private readonly ComponentPool<ScaleComp> _scalePool;
		private readonly ComponentPool<PositionComp> _positionPool;
		private readonly ComponentPool<IsBulletProjectileComp> _isBulletPool;
		private readonly ComponentPool<IsAoeProjectileComp> _isAoePool;

		//TODO MOVE US
		private readonly AudioClip[] _hitSounds;
		private readonly AudioClip[] _explosionSounds;
		private float _hitSoundTime;
		private float _explosionSoundTime;

		private PhysicsScene2D _physicsScene;

		private bool _canHitPlayer, _aoeCanHitPlayer;
		private float _bulletDamageMultiplier;

		private readonly ContactFilter2D _contactFilter;

		private readonly Collider2D[] _collisionResultsArray;
		private readonly Collider2D[] _collisionAoEResultsArray;
		private readonly Collider2D[] _knockBackResultsArray;

		private readonly Vector2 _extraProjectileScale = new Vector2(0.2f, 0.2f);
		private const float DistanceToHit = 5f;

		public ProjectileCollisionSystem(World world) : base(world, SystemParameters.UsePlayer)
		{
			_contactFilter = new ContactFilter2D();
			_collisionResultsArray = new Collider2D[5];
			_collisionAoEResultsArray = new Collider2D[100];
			_knockBackResultsArray = new Collider2D[5];

			_scalePool = ComponentManager<ScaleComp>.GetOrCreate(World.WorldId);
			_positionPool = ComponentManager<PositionComp>.GetOrCreate(World.WorldId);
			_isBulletPool = ComponentManager<IsBulletProjectileComp>.GetOrCreate(World.WorldId);
			_isAoePool = ComponentManager<IsAoeProjectileComp>.GetOrCreate(World.WorldId);

			_hitSounds = Resources.LoadAll<AudioClip>("Audio/SoundEffects/Damage").ToArray();
			_explosionSounds = Resources.LoadAll<AudioClip>("Audio/SoundEffects/Explosion").Where(s => s.name.Contains("explosion"))
				.ToArray();
		}

		protected override void PreUpdate(float state)
		{
			ref readonly var upgradesComp = ref World.Get<UpgradesComp>();
			_canHitPlayer = upgradesComp.CanHitPlayer.IsOn();
			_aoeCanHitPlayer = upgradesComp.CanHitPlayerWithAoe.IsOn();
			_bulletDamageMultiplier = upgradesComp.BulletDamageMultiplier;
			_physicsScene = Physics2D.defaultPhysicsScene;

			if (_hitSoundTime > 0)
				_hitSoundTime -= state;
			if (_explosionSoundTime > 0)
				_explosionSoundTime -= state;
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			var playerPosition = _positionPool.Get(Player.EntityId).Value;
			double playerScaleMagnitude = _scalePool.Get(Player.EntityId).Magnitude;
			//ref readonly var playerEvasion = ref player.Get<EvasionComp>();
			//var playerDamageEvents = player.Get<DamageEventsComp>().DamageEvents;
			bool playerDeflects = Player.Has<DoDeflectProjectilesComp>();
			var playerDirection = Player.Get<VelocityComp>().Direction;
			bool playerUnHittable = Player.Has<UnHittableComp>();
			ref readonly var stats = ref Player.Get<StatsComp>();

			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var projectile = ref entities[i];
				var position = _positionPool.Get(projectile.EntityId).Value;
				ref readonly var scale = ref _scalePool.Get(projectile.EntityId);
				bool isBullet = _isBulletPool.Has(projectile.EntityId);
				bool isAoE = _isAoePool.Has(projectile.EntityId);
				ref var piercing = ref projectile.Get<PiercingComp>();

				if (_canHitPlayer
				    && !playerUnHittable
				    && Vector2.Distance(position, projectile.Get<StartPositionComp>().Value) > 40f //Travelled 40 units from start
				    && Vector2.Distance(position, playerPosition) <= DistanceToHit + playerScaleMagnitude * 2.5d + scale.Magnitude)
				{
					if (playerDeflects)
					{
						//TODO Deflection cooldown?
						//TODO Should player only deflect if has deflection and can be hit?

						if (piercing.EntitiesHit.Contains(Player)) //Don't deflect again from the player, just pass through?
							continue;

						piercing.EntitiesHit.Add(Player);

						ref var velocity = ref projectile.Get<VelocityComp>();
						velocity.Direction = Vector2.Reflect(velocity.Direction, playerDirection);
						velocity.Direction.Normalize();
						continue; //Skip damaging enemies as well?
					}

					if (UnitHit(in projectile, in stats, position, isBullet, isAoE, ref piercing, in Player, playerPosition))
						continue; //If hit player, don't check for other units this frame
				}

				HitEnemies(in projectile, in stats, position, scale.Value + _extraProjectileScale, isBullet, isAoE, ref piercing,
					playerPosition);
			}
		}

		private void HitEnemies(in Entity projectile, in StatsComp stats, Vector2 position, Vector2 scale, bool isBullet, bool isAoE,
			ref PiercingComp piercing, Vector2 playerPosition)
		{
			int hits = _physicsScene.OverlapBox(position, scale, 0f, _contactFilter, _collisionResultsArray);
			for (int i = 0; i < hits; i++)
			{
				ref readonly var enemy = ref _collisionResultsArray[i].gameObject.GetComponent<EnemyReference>().Entity;

				if (enemy.Has<DoDeflectProjectilesComp>())
				{
					//TODO Deflection cooldown?
					//TODO Sniper projectiles should not be deflected, and/or certain velocity skips deflection

					if (piercing.EntitiesHit.Contains(enemy)) //Don't deflect again from same enemy, just pass through?
						continue;

					piercing.EntitiesHit.Add(enemy);

					ref var velocity = ref projectile.Get<VelocityComp>();
					velocity.Direction = Vector2.Reflect(velocity.Direction, enemy.Get<VelocityComp>().Direction);
					velocity.Direction.Normalize();
					//TODO Rotation component?
					projectile.Get<GameObjectComp>().Value.transform.rotation = Quaternion.Euler(0f, 0f,
						Mathf.Atan2(velocity.Direction.y, velocity.Direction.x) * Mathf.Rad2Deg - 90f);
					break; //Skip damaging enemies as well?
				}

				if (enemy.Has<UnHittableComp>())
					continue;

				if (EnemyHit(in projectile, stats, position, isBullet, isAoE, ref piercing, in enemy, playerPosition))
					break;
			}
		}

		private bool EnemyHit(in Entity projectile, in StatsComp stats, Vector2 position, bool isBullet, bool isAoE,
			ref PiercingComp piercing, in Entity enemy, Vector2 playerPosition)
		{
			if (!UnitHit(in projectile, in stats, position, isBullet, isAoE, ref piercing, in enemy, playerPosition))
				return false;

			if (_hitSoundTime < 1)
			{
				_hitSoundTime += 0.15f;
				World.Get<AudioComp>().Value.pitch = 0.8f + Random.Range(-0.1f, 0.1f) + (enemy.Get<ScaleComp>().Magnitude - 3f) / 42f;
				World.Get<AudioComp>().Value.PlayOneShot(_hitSounds[Random.Range(0, _hitSounds.Length)], 0.8f);
			}

			World.Get<ScoreComp>().EnemiesHitWithProjectiles++;

			//Basic knockback
			if (enemy.TryGet(out VelocityComp velocity))
			{
				//if any enemies behind the knockBacked enemy, push them as well
				var transform = enemy.Get<GameObjectComp>().Value.transform;
				var newPosition = transform.position - (Vector3)(velocity.Direction * (25f / enemy.Get<ScaleComp>().Magnitude));
				transform.position = newPosition;
				int knockBackHits =
					_physicsScene.OverlapBox(newPosition, transform.localScale, 0f, _contactFilter, _knockBackResultsArray);
				for (int i = 0; i < knockBackHits; i++)
				{
					ref readonly var enemyToKnockBack = ref _knockBackResultsArray[i].gameObject.GetComponent<EnemyReference>().Entity;
					if (enemyToKnockBack == enemy || !enemyToKnockBack.Has<VelocityComp>())
						continue;

					enemyToKnockBack.Get<GameObjectComp>().Value.transform.position -=
						(Vector3)(enemyToKnockBack.Get<VelocityComp>().Direction * (10f / enemyToKnockBack.Get<ScaleComp>().Magnitude));
				}
			}

			//TODO Apply all effects, also in a better way
			if (projectile.Has<BurningDataComp>())
			{
				ref readonly var burningData = ref projectile.Get<BurningDataComp>();
				enemy.Set(new BurningComp { Duration = burningData.Duration, Damage = burningData.Damage });
			}

			//if (projectile.Has<PoisonDataComp>())
			if (projectile.TryGet<ColdDataComp>(out var coldData))
			{
				enemy.Set(new ColdComp { Duration = coldData.Duration, SlowMultiplier = coldData.SlowMultiplier });
			}

			return true;
		}

		private bool UnitHit(in Entity projectile, in StatsComp stats, Vector2 position, bool isBullet, bool isAoE,
			ref PiercingComp piercing, in Entity entity, Vector2 playerPosition)
		{
			if (piercing.EntitiesHit.Contains(entity))
				return false;

			//Even if the unit dodges the attack. We save it as hit, so when the projectile is traveling through the unit, we won't check again. Not 100% ideal
			piercing.EntitiesHit.Add(entity);
			if (!entity.Has<DoIgnoreEvasion>() && entity.Get<EvasionComp>().Roll())
				return false;

			if (isBullet)
			{
				ref readonly var damage = ref projectile.Get<DamageComp>();
				entity.Get<DamageEventsComp>().DamageEvents.Add(new DamageEvent
				{
					Source = projectile, DamageType = damage.DamageType,
					Damage = (damage.Value + stats.ExtraBulletDamage) * _bulletDamageMultiplier
				});
			}

			if (isAoE)
			{
				float aoeDamage = projectile.Get<AoEDamageComp>().Value;
				float radius = projectile.Get<RadiusComp>().Value;

				//TODO to a list/collection that gets added ton sent and cleared each frame
				World.Publish(new AoEAnimationEvent(AnimationType.Explosion, position, radius));
				if (_explosionSoundTime < 1)
				{
					_explosionSoundTime += 0.35f;
					World.Get<AudioComp>().Value.PlayOneShot(_explosionSounds[Random.Range(0, _explosionSounds.Length)], 0.7f);
				}

				if (_aoeCanHitPlayer && Vector2.Distance(position, playerPosition) <= radius)
				{
					Player.Get<DamageEventsComp>().DamageEvents.Add(new DamageEvent
						{ Source = projectile, DamageType = DamageType.Explosive, Damage = aoeDamage });
				}

				//TODO Apply all effects, also in a better way
				bool hasBurning = projectile.TryGet(out BurningDataComp burningData);
				bool hasCold = projectile.TryGet(out ColdDataComp coldData);
				//if (projectile.Has<PoisonDataComp>())

				int aoeHits = _physicsScene.OverlapCircle(position, radius, _contactFilter, _collisionAoEResultsArray);

				for (int i = 0; i < aoeHits; i++)
				{
					ref readonly var aoeEnemy = ref _collisionAoEResultsArray[i].gameObject.GetComponent<EnemyReference>().Entity;
					aoeEnemy.Get<DamageEventsComp>().DamageEvents.Add(new DamageEvent()
						{ Source = projectile, DamageType = DamageType.Explosive, Damage = aoeDamage });

					if (hasBurning)
						aoeEnemy.Set(new BurningComp { Duration = burningData.Duration, Damage = burningData.Damage });
					if (hasCold)
						aoeEnemy.Set(new ColdComp { Duration = coldData.Duration, SlowMultiplier = coldData.SlowMultiplier });
				}
			}

			piercing.HitsLeft--;
			return true;
		}

		internal void EnemyHit(in Entity projectile, in Entity enemy)
		{
			var statsComp = default(StatsComp);
			EnemyHit(in projectile, in statsComp, _positionPool.Get(projectile.EntityId).Value, _isBulletPool.Has(projectile.EntityId),
				_isAoePool.Has(projectile.EntityId), ref projectile.Get<PiercingComp>(), in enemy, Vector2.zero);
		}
	}
}