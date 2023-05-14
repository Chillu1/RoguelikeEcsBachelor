using System;
using DefaultEcs;
using DefaultEcs.System;
using ElRaccoone.Tweens;
using RoguelikeEcs.Core.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace RoguelikeEcs.Core
{
	[With(typeof(HealthBarComp))]
	public class HealthBarSystem : AEntitySetSystem<float>
	{
		private readonly Camera _camera;
		private static readonly GameObject healthBarPrefab;
		private static Transform _enemyHealthBarsTransform;

		private readonly TMP_Text _playerHealthText;

		static HealthBarSystem()
		{
			healthBarPrefab = Resources.Load<GameObject>("Prefabs/HealthBar");
		}

		public HealthBarSystem(World world, Camera camera) : base(world)
		{
			if (Core.IsTestMode)
				return;

			_camera = camera;

			var canvas = Object.FindObjectOfType<Canvas>();
			_enemyHealthBarsTransform = new GameObject("EnemyHealthBars").transform;
			_enemyHealthBarsTransform.SetParent(canvas.transform, false);
			_enemyHealthBarsTransform.SetAsFirstSibling();

			_playerHealthText = canvas.transform.Find("Health").GetComponent<TMP_Text>();
		}

		protected override void Update(float state, in Entity entity)
		{
			var position = entity.Get<GameObjectComp>().Value.transform.position;
			ref var healthBar = ref entity.Get<HealthBarComp>();

			healthBar.Transform.position = _camera.WorldToScreenPoint(position + Vector3.up * 15f);

			ref readonly var health = ref entity.Get<HealthComp>();
			float healthPercent = health.Percent;

			healthBar.DamageTakenTimer += state;

			if (Math.Abs(healthBar.LastHealthPercent - healthPercent) < 0.001f && healthBar.LastMaxHealth == health.Max)
				return;

			if (entity.Has<IsPlayerComp>())
				_playerHealthText.text = $"Health: {health.Current:f1}/{health.Max:f1}";

			float maxMultiplier = health.Max / 100f;
			healthBar.HealthBarLinesImage.pixelsPerUnitMultiplier = healthBar.PixelsPerUnitMultiplier * maxMultiplier;
			healthBar.HealthBarFgImage.fillAmount = healthPercent;
			healthBar.LastHealthPercent = healthPercent;
			healthBar.LastMaxHealth = health.Max;

			if (healthBar.DamageTakenTimer < 0.3f)
				return;

			if (healthBar.HealthBarDamageTakenImage.TryGetTween(out var tween))
			{
				//Cancel the tween and delay the timer so new tween can be made with new health percent
				tween.Cancel();
				healthBar.DamageTakenTimer = 0.2f;
				return;
			}

			healthBar.HealthBarDamageTakenImage.TweenImageFillAmount(healthPercent, 0.5f);
			healthBar.DamageTakenTimer = 0f;
		}

		public static HealthBarComp CreateNewHealthBar(float scale = 1f, bool isEnemy = true)
		{
			var healthBarComp = new HealthBarComp();

			var canvas = Object.FindObjectOfType<Canvas>().transform;
			var healthBar = Object.Instantiate(healthBarPrefab, isEnemy ? _enemyHealthBarsTransform : canvas, false);
			healthBar.transform.SetAsFirstSibling();

			healthBarComp.Transform = healthBar.GetComponent<RectTransform>();
			healthBarComp.HealthBarLinesImage = healthBar.transform.Find("HealthBarLines").GetComponent<Image>();
			healthBarComp.HealthBarFgImage = healthBar.transform.Find("HealthBarFg").GetComponent<Image>();
			healthBarComp.HealthBarDamageTakenImage = healthBar.transform.Find("HealthBarDamageTaken").GetComponent<Image>();

			if (isEnemy)
			{
				healthBarComp.Transform.localScale = healthBarComp.HealthBarLinesImage.transform.localScale =
					healthBarComp.HealthBarFgImage.transform.localScale = healthBarComp.HealthBarDamageTakenImage.transform.localScale =
						healthBar.transform.Find("HealthBarBg").transform.localScale = Vector3.one * (0.7f * scale);
				healthBarComp.HealthBarFgImage.color = new Color(0.85f, 0.11f, 0.31f);
			}

			healthBarComp.PixelsPerUnitMultiplier = healthBarComp.HealthBarLinesImage.pixelsPerUnitMultiplier;

			healthBarComp.LastHealthPercent = 1f;

			return healthBarComp;
		}
	}
}