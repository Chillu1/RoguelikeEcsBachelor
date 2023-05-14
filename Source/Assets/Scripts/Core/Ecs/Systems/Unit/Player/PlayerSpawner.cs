using System.Collections.Generic;
using System.Linq;
using DefaultEcs;
using RoguelikeEcs.Core.Utilities;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	public static class PlayerSpawner
	{
		public static float Health { get; private set; }

		public static Entity Spawn(World world, CharacterInfo characterInfo, int startPosX, int startPosY, float health = 100f)
		{
			var player = world.CreateEntity();

			player.Set<IsPlayerComp>();
			player.Set<IsCameraTargetComp>();
			player.Set<PlayerMouseClickComp>();
			player.Set<StatsComp>();
			player.Set(HealthBarSystem.CreateNewHealthBar(1f, false));

			player.Set(new CharacterTypeComp { Value = characterInfo.Type });
			player.Set(new PositionComp { Value = new Vector2(startPosX, startPosY) });
			player.Set(new VelocityComp { Speed = 200f });
			Vector2 scale = new Vector2(3f, 3f);
			player.Set(new ScaleComp { Value = scale, Magnitude = scale.GetMagnitude() });

			Health = health;
			player.Set(new HealthComp() { Current = health, Max = health });
			player.Set(new HealthRegenComp { Value = 0.2f });
			player.Set(new PreviousHealthComp() { Value = health });
			player.Set(new AttackComp() { Cooldown = 0.5f });
			player.Set(new AttackDataComp());
			player.Set(ResistancesComp.Default);
			player.Set<EvasionComp>();
			player.Set(new DamageEventsComp() { DamageEvents = new List<DamageEvent>(15) });
			player.Set(new CircleEffectDataComp { Cooldown = 1f, CooldownTimer = 1f });

			player.Set(new MultiplierComp()
				{ BaseMultipliers = new float[] { 1, 1, 1, 1 }, MultipliersList = new List<MultiplierData>(5) });
			player.Set(new RuptureDataComp { Percentage = 0.2f });

			player.Set<HomingDataComp>();

			var playerCurse = new PlayerCurseComp() { Duration = 5f };
			player.Set(playerCurse);

			var go = new GameObject("Player");
			var wandGo = new GameObject("Wand");
			wandGo.transform.parent = go.transform;
			var wand = new WandComp() { Child = wandGo };
			if (!Core.IsTestMode)
			{
				var child = new GameObject("PlayerSprite");
				child.transform.parent = go.transform;
				var renderer = child.AddComponent<SpriteRenderer>();
				renderer.sprite = Resources.Load<Sprite>("Textures/Player" + characterInfo.Type);
				renderer.sortingOrder = 2;
				renderer.material = Resources.Load<Material>("Materials/PlayerMaterial");
				if (renderer.sprite == null)
				{
					Debug.LogWarning("Player sprite not found: " + characterInfo.Type);
					renderer.sprite = Resources.Load<Sprite>("Textures/PlayerDefault");
				}

				player.Set(new RendererComp() { Value = renderer, Child = child });

				var wandRenderer = wandGo.AddComponent<SpriteRenderer>();
				wandRenderer.sprite = Resources.Load<Sprite>("Textures/Wand");
				wandRenderer.sortingOrder = 3;
				wand.Renderer = wandRenderer;
				var wandAnimator = wandGo.AddComponent<Animator>();
				wandAnimator.runtimeAnimatorController =
					Resources.Load<RuntimeAnimatorController>("Animations/WandSmall1");
				wandAnimator.speed = 0.7f;
				wand.Animator = wandAnimator;

				var playerShadow = new GameObject("PlayerShadow");
				playerShadow.transform.parent = go.transform;
				playerShadow.transform.localPosition = new Vector3(0, -0.8f, 0);
				var shadowRenderer = playerShadow.AddComponent<SpriteRenderer>();
				shadowRenderer.sprite = Resources.Load<Sprite>("Textures/Shadow");
				shadowRenderer.sortingOrder = 2;
				player.Set(new ShadowRendererComp { Value = shadowRenderer });
			}

			var audioSource = go.AddComponent<AudioSource>();
			audioSource.volume = 0.13f;
			player.Set(new AudioComp
			{
				Value = audioSource,
				Clips = Resources.LoadAll<AudioClip>("Audio/SoundEffects/Shooting/").Where(s => s.name.Contains("shoot")).ToArray()
			});

			player.Set(wand);

			go.transform.localScale = scale;
			var rb = go.AddComponent<Rigidbody2D>();
			rb.bodyType = RigidbodyType2D.Kinematic;
			rb.gravityScale = 0;
			rb.interpolation = RigidbodyInterpolation2D.Interpolate;

			player.Set(new GameObjectComp() { Value = go });
			player.Set(new RigidbodyComp() { Value = rb });

			characterInfo.Apply(ref player.Get<MultiplierComp>(), ref world.Get<UpgradesComp>(), ref player.Get<StatsComp>());

			return player;
		}
	}
}