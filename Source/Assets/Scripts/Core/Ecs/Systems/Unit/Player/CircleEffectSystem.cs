using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	[With(typeof(CircleEffectComp))]
	public sealed class CircleEffectSystem : CustomEntitySetSystem
	{
		private readonly ContactFilter2D _contactFilter;
		private readonly Collider2D[] _colliders;

		private readonly Color _colorOne;
		private readonly Color _colorTwo;

		private readonly AudioClip _circleSound;

		private PhysicsScene2D _physicsScene;
		private bool _hasDeathMark;

		private float _aoeRadiusMultiplier;

		public CircleEffectSystem(World world) : base(world, SystemParameters.UsePlayer)
		{
			_contactFilter = new ContactFilter2D();
			_colliders = new Collider2D[100];

			_colorOne = Color.green;
			_colorTwo = new Color(0.25f, 0.5f, 0.25f, 0.3f);

			_circleSound = Resources.Load<AudioClip>("Audio/SoundEffects/Circle/circle04");
		}

		protected override void PreUpdate(float state)
		{
			_physicsScene = Physics2D.defaultPhysicsScene;

			ref readonly var upgrades = ref World.Get<UpgradesComp>();
			_hasDeathMark = upgrades.HasDeathMark.IsOn();

			_aoeRadiusMultiplier = upgrades.AoERadiusMultiplier;
		}

		protected override void Update(float state, in Entity entity)
		{
			ref var circleEffect = ref entity.Get<CircleEffectComp>();
			circleEffect.Timer += state;

			if (circleEffect.Timer >= circleEffect.Duration)
			{
				entity.Set<DestroyComp>();
				return;
			}

			//entity.Get<RendererComp>().Value.color = Color.Lerp(_colorOne, _colorTwo, Mathf.PingPong(Time.time, 1f));

			circleEffect.EffectTimer += state;
			if (circleEffect.EffectTimer < circleEffect.EffectInterval)
				return;

			World.Get<AudioComp>().Value.PlayOneShot(_circleSound, 0.1f);
			circleEffect.EffectTimer = 0;

			ref readonly var position = ref entity.Get<PositionComp>();
			ref readonly var radius = ref entity.Get<RadiusComp>();
			ref readonly var damage = ref entity.Get<DamageComp>();
			var upgrades = World.Get<UpgradesComp>();
			bool hasDamage = entity.Has<DamageComp>();


			var hasCold = false;
			var coldData = new ColdDataComp();
			if (entity.Has<ColdDataComp>())
			{
				hasCold = true;
				coldData = entity.Get<ColdDataComp>();
			}

			bool hasBurn = false;
			var burnData = new BurningDataComp();
			if (entity.Has<BurningDataComp>())
			{
				hasBurn = true;
				burnData = entity.Get<BurningDataComp>();
			}

			bool hasPoison = false;
			var poisonData = new PoisonDataComp();
			if (entity.Has<PoisonDataComp>())
			{
				hasPoison = true;
				poisonData = entity.Get<PoisonDataComp>();
			}

			var playerPosition = Player.Get<PositionComp>();
			var playerHealth = Player.Get<HealthComp>();
			ref var playerCurseComp = ref Player.Get<PlayerCurseComp>();
			var distance = Vector2.Distance(playerPosition.Value, position.Value);
			var circleHealValue = 0.2f;

			int hits = _physicsScene.OverlapCircle(position.Value, radius.Value * _aoeRadiusMultiplier, _contactFilter, _colliders);

			World.Publish(new AoEAnimationEvent(AnimationType.Circle, position.Value, radius.Value));

			for (int i = 0; i < hits; i++)
			{
				ref readonly var hitEntity = ref _colliders[i].GetComponent<EnemyReference>().Entity;

				if (hasDamage)
				{
					hitEntity.Get<DamageEventsComp>().DamageEvents.Add(new DamageEvent()
						{ Source = entity, Damage = damage.Value, DamageType = damage.DamageType });
				}

				if (hasCold)
				{
					var cold = new ColdComp();
					cold.SlowMultiplier = coldData.SlowMultiplier;
					cold.Duration = coldData.Duration;
					hitEntity.Set(cold);
				}

				if (hasBurn)
				{
					var burn = new BurningComp();
					burn.Duration = burnData.Duration;
					burn.Damage = burnData.Damage;
					hitEntity.Set(burn);
				}

				if (hasPoison)
				{
					if (hitEntity.Has<PoisonedComp>())
					{
						var hitPoisonComp = hitEntity.Get<PoisonedComp>();
						hitPoisonComp.Stacks++;
						hitPoisonComp.Timer = poisonData.Duration;
						hitPoisonComp.Damage = poisonData.Damage;
					}
					else
					{
						var poison = new PoisonedComp();
						poison.Damage = poisonData.Damage;
						poison.Timer = poisonData.Duration;
						hitEntity.Set(poison);
					}
				}

				if (upgrades.HasCircleHealPlayer)
				{
					if (distance <= radius.Value)
						playerHealth.Current += circleHealValue;
				}

				// Player Curse
				if (distance <= radius.Value)
					playerCurseComp.Timer = playerCurseComp.Duration;


				//TODO And not immune to death mark (giants? undead?)
				if (_hasDeathMark && !hitEntity.Has<DeathMarkComp>() && !hitEntity.Has<IsBossEnemy>())
					hitEntity.Set(new DeathMarkComp { Duration = 20f });
			}
		}
	}
}