using System;
using DefaultEcs;
using DefaultEcs.System;
using ElRaccoone.Tweens;
using ElRaccoone.Tweens.Core;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RoguelikeEcs.Core
{
	[With(typeof(IsAnimationComp))]
	public sealed class RenderingSystem : AEntitySetSystem<float>
	{
		private readonly RuntimeAnimatorController _explosionAnimationController;
		private readonly RuntimeAnimatorController _circleAnimationController;
		private readonly RuntimeAnimatorController _grabAnimationController;
		private readonly TMP_FontAsset _damageFont;

		private readonly Transform _animationParent;

		private const int MaxDamageDealtCount = 20;
		private int _damageDealtCount;

		private float _totalDamageDealt, _totalDamageCount;
		private float _avgDamageDealt;

		public RenderingSystem(World world) : base(world)
		{
			_explosionAnimationController = Resources.Load<RuntimeAnimatorController>("Animations/Explosion1");
			_circleAnimationController = Resources.Load<RuntimeAnimatorController>("Animations/Circle1");
			_grabAnimationController = Resources.Load<RuntimeAnimatorController>("Animations/Grab1");
			_damageFont = Resources.Load<TMP_FontAsset>("Fonts/Thintel SDF");

			_animationParent = new GameObject("Animations").transform;

			_avgDamageDealt = 50;

			world.Subscribe<AoEAnimationEvent>(OnAoEAnimation);
			world.Subscribe<RangedAttackAnimationEvent>(OnRangedAttackAnimation);
			world.Subscribe<DamageDealtEvent>(OnDamageDealt);
		}

		private void OnAoEAnimation(in AoEAnimationEvent message)
		{
			var entity = World.CreateEntity();
			entity.Set<IsAnimationComp>();
			float timer;

			var go = new GameObject("AoEAnimation");
			var renderer = go.AddComponent<SpriteRenderer>();
			renderer.sortingOrder = 1;
			var animator = go.AddComponent<Animator>();
			switch (message.AnimationType)
			{
				case AnimationType.Explosion:
					animator.runtimeAnimatorController = _explosionAnimationController;
					animator.speed = 6f;
					float length = _explosionAnimationController.animationClips[0].length;
					timer = length / animator.speed;
					go.TweenLocalScale(Vector3.one * message.Radius * 3.5f, timer * 0.5f).SetEase(EaseType.QuadOut)
						.TweenLocalScale(Vector3.one * message.Radius * 2.4f, timer * 0.5f).SetEase(EaseType.QuadIn)
						.SetDelay(timer * 0.5f);
					break;
				case AnimationType.Circle:
					animator.runtimeAnimatorController = _circleAnimationController;
					animator.speed = 6f;
					timer = _circleAnimationController.animationClips[0].length / animator.speed;
					renderer.color = new Color(0.5f, 1f, 1f, 0.65f);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			go.transform.position = message.Position;
			go.transform.localScale = Vector3.one * message.Radius * 2.4f;
			go.transform.SetParent(_animationParent);

			entity.Set(new TimerComp { Value = timer });
			entity.Set(new AnimationComp { Value = animator });
			entity.Set(new GameObjectComp { Value = go });
		}

		private void OnRangedAttackAnimation(in RangedAttackAnimationEvent message)
		{
			var entity = World.CreateEntity();
			entity.Set<IsAnimationComp>();
			entity.Set(new TimerComp { Value = 1f });

			var go = new GameObject("RangedAttackAnimation");
			go.AddComponent<SpriteRenderer>();
			var animator = go.AddComponent<Animator>();
			animator.runtimeAnimatorController = _grabAnimationController;
			animator.speed = 0.8f;

			go.transform.position = message.Position;
			go.transform.localScale = Vector3.one * message.Radius * 2.4f;
			go.transform.SetParent(_animationParent);
			go.transform.rotation = Quaternion.Euler(0, 0, message.Angle);

			entity.Set(new AnimationComp { Value = animator });
			entity.Set(new GameObjectComp { Value = go });
		}

		private void OnDamageDealt(in DamageDealtEvent message)
		{
			if (_damageDealtCount >= MaxDamageDealtCount)
				return;

			_totalDamageDealt += message.TotalDamage;
			_avgDamageDealt = _totalDamageDealt / ++_totalDamageCount;
			//TODO Reset every wave?

			//TODO Limiting/ignoring only the low damage instances
			//TODO Maybe pooling

			var position = message.Entity.Get<PositionComp>().Value;
			var go = new GameObject("DamageDealt");
			var goTransform = go.transform;
			goTransform.parent = _animationParent;
			goTransform.position = position;

			var text = go.AddComponent<TextMeshPro>();
			text.text = message.TotalDamage.ToString("0");
			text.alignment = TextAlignmentOptions.Center;
			text.enableAutoSizing = true;
			text.enableWordWrapping = false;
			text.enableKerning = false;
			text.enableCulling = false;
			text.extraPadding = false;
			text.enableVertexGradient = false;
			text.richText = false;
			text.font = _damageFont;
			text.UpdateFontAsset();

			text.sortingOrder = (int)message.TotalDamage;

			//TODO Better scaling
			//0-50 = white. 50-150 = yellow. 150-250 = orange. 250-350 = red. 350+ = dark red
			var destColor = Color.Lerp(Color.white, Color.yellow, message.TotalDamage / (_avgDamageDealt * 0.5f));
			destColor = Color.Lerp(destColor, Color.red, message.TotalDamage / _avgDamageDealt);

			text.color = Color.Lerp(Color.white, destColor, 0.3f);
			float scale = 4f;
			var startScale = Vector3.one * (Mathf.Lerp(0.8f, 1.2f, message.TotalDamage / (_avgDamageDealt * 0.5f)) * scale);

			float duration = Mathf.Lerp(0.5f, 1.4f, message.TotalDamage / (_avgDamageDealt * 0.5f));

			if (message.TotalDamage < 0.5f) //Funny 0 damage big numbers
			{
				startScale = Vector3.one * (1.2f * scale);
				text.color = Color.yellow;
				destColor = Color.red;
				duration = 1f;
			}

			go.transform.localScale = startScale * 0.5f;

			go.TweenPositionY(position.y + 35, 0.7f * duration)
				.TweenGraphicColor(destColor, 0.7f * duration).SetEase(EaseType.QuadIn)
				.TweenLocalScale(startScale, 0.3f * duration).SetEase(EaseType.QuadOut)
				.TweenLocalScale(Vector3.one * 0.5f, 0.4f * duration).SetEase(EaseType.QuadIn).SetDelay(0.6f * duration)
				.TweenGraphicAlpha(0f, 1f * duration).SetEase(EaseType.QuadIn)
				.SetOnComplete(() =>
				{
					Object.Destroy(go);
					_damageDealtCount--;
				});

			_damageDealtCount++;
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];
				ref var timer = ref entity.Get<TimerComp>();

				timer.Value -= state;

				if (timer.Value <= 0f)
					entity.Set<DestroyComp>();
			}
		}
	}
}