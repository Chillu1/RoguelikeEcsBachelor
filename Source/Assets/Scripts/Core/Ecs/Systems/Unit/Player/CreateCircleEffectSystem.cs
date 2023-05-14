using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	[With(typeof(CircleEffectDataComp))]
	public sealed class CreateCircleEffectSystem : CustomEntitySetSystem
	{
		private readonly Sprite _circleSprite;
		private readonly SpriteRenderer _startPointSpriteRenderer;
		private readonly Color _startingPointColor;

		private static readonly Vector2 defaultStartPosition = new Vector2(float.MinValue, float.MinValue);
		private Vector2 _startPosition = defaultStartPosition;
		private Vector2 _minXMaxY;
		private Vector2 _maxXMaxY;
		private Vector2 _maxXMinY;
		private Vector2 _minXMinY;

		public CreateCircleEffectSystem(World world) : base(world, SystemParameters.UsePlayer)
		{
			_circleSprite = Resources.Load<Sprite>("Textures/Circle2");
			_startPointSpriteRenderer = new GameObject("CircleEffectStartPoint").AddComponent<SpriteRenderer>();
			_startPointSpriteRenderer.sprite = Resources.Load<Sprite>("Textures/Circle2");

			_startPointSpriteRenderer.color = new Color(0.54f, 0f, 0.09f);
			_startPointSpriteRenderer.transform.localScale = new Vector3(4f, 4f, 1f);
		}

		protected override void Update(float state, in Entity entity)
		{
			ref var circleEffect = ref entity.Get<CircleEffectDataComp>();

			circleEffect.CooldownTimer -= state;
			if (circleEffect.CooldownTimer > 0)
				return;

			circleEffect.SaveTimer -= state;
			if (circleEffect.SaveTimer < 0)
			{
				var position = entity.Get<PositionComp>().Value;
				if (_startPosition == defaultStartPosition)
				{
					_startPosition = position;
					_startPointSpriteRenderer.enabled = true;
					_startPointSpriteRenderer.transform.position = _startPosition;
				}

				if (position.x < _minXMaxY.x && position.y > _minXMaxY.y)
					_minXMaxY = position;
				if (position.x > _maxXMaxY.x && position.y > _maxXMaxY.y)
					_maxXMaxY = position;
				if (position.x > _maxXMinY.x && position.y < _maxXMinY.y)
					_maxXMinY = position;
				if (position.x < _minXMinY.x && position.y < _minXMinY.y)
					_minXMinY = position;

				circleEffect.SaveTimer = 0.1f;
			}

			if (circleEffect.CooldownTimer >= -1f)
				return;

			if (Vector2.Distance(entity.Get<PositionComp>().Value, _startPosition) < 22f)
			{
				//TODO Radius and most likely center are wrong
				//Distance between 4 points
				float radius = (Vector2.Distance(_minXMaxY, _maxXMaxY) + Vector2.Distance(_maxXMaxY, _maxXMinY) +
				                Vector2.Distance(_maxXMinY, _minXMinY) + Vector2.Distance(_minXMinY, _minXMaxY)) / 8f;

				Vector2 center = new Vector2((_minXMaxY.x + _maxXMaxY.x + _maxXMinY.x + _minXMinY.x) / 4f,
					(_minXMaxY.y + _maxXMaxY.y + _maxXMinY.y + _minXMinY.y) / 4f);

				CreateCircleEffectEntity(center, radius);

				circleEffect.CooldownTimer = circleEffect.Cooldown;

				_startPosition = defaultStartPosition;
				_startPointSpriteRenderer.enabled = false;
				_minXMaxY = new Vector2(float.MaxValue, float.MinValue);
				_maxXMaxY = new Vector2(float.MinValue, float.MinValue);
				_maxXMinY = new Vector2(float.MinValue, float.MaxValue);
				_minXMinY = new Vector2(float.MaxValue, float.MaxValue);
			}
		}

		private void CreateCircleEffectEntity(Vector2 center, float radius)
		{
			ref readonly var stats = ref Player.Get<StatsComp>();
			var entity = World.CreateEntity();

			entity.Set(new PositionComp { Value = center });
			entity.Set(new RadiusComp { Value = radius });
			entity.Set(new CircleEffectComp { Duration = 5f, EffectInterval = 1f });

			//TODO More/different effects, poison, cold (slow), fire, stun?
			entity.Set(CritComp.Default);
			entity.Set<LifestealComp>();

			ref var upgrades = ref World.Get<UpgradesComp>();
			if (upgrades.HasCircleDamage)
			{
				entity.Set(new DamageComp { Value = (10 + stats.ExtraDamage) * upgrades.CircleDamageModifier });
			}

			if (upgrades.HasCircleCold)
			{
				entity.Set(new ColdDataComp
				{
					Duration = 5f * upgrades.CircleEffectDurationMultiplier,
					SlowMultiplier = upgrades.CircleColdSlowMultiplier
				});
			}

			if (upgrades.HasCircleBurn)
			{
				entity.Set(new BurningDataComp
				{
					Duration = 10f * upgrades.CircleEffectDurationMultiplier,
					Damage = 5f * upgrades.CircleBurnDamageMultiplier
				});
			}

			if (upgrades.HasCirclePoison)
			{
				entity.Set(new PoisonDataComp
				{
					Duration = 15f * upgrades.CircleEffectDurationMultiplier,
					Damage = 3f * upgrades.CirclePoisonDamageMultiplier
				});
			}

			// var gameObject = new GameObject("CircleEffect");
			// var spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
			// spriteRenderer.sprite = _circleSprite;
			// var rendererTransform = spriteRenderer.transform;
			// rendererTransform.position = center;
			// rendererTransform.transform.localScale = new Vector3(radius / spriteRenderer.sprite.bounds.size.x + 2,
			// 	radius / spriteRenderer.sprite.bounds.size.y + 2, 1);
			//
			// entity.Set(new GameObjectComp { Value = gameObject });
			// entity.Set(new RendererComp { Value = spriteRenderer });
		}
	}
}