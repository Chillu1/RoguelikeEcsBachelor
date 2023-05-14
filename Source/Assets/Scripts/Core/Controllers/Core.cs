using System.Collections.Generic;
using DefaultEcs;
using RoguelikeEcs.Core.Utilities;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	public static class Core
	{
		public static bool IsTestMode { get; private set; }

		public static CoreSystem<float> GetUISystem(World world, PauseController pauseController)
		{
			var coreSystem = new CoreSystem<float>(
				new KeyBindSystem(world, pauseController)
			);

			return coreSystem;
		}

		public static CoreSystem<float> GetGameLoopSystem(World world, AreaSizeType areaSizeType, DifficultyType difficulty,
			DataController dataController, bool testMode = false)
		{
			IsTestMode = testMode;

			var areaSize = areaSizeType.GetSize();

			var camera = Camera.main;
			float projectileOffset = 150;
			var bounds = new Bounds(
				new Vector2(-areaSize.x, -areaSize.y),
				new Vector2(areaSize.x, areaSize.y),
				new Vector2(-areaSize.x - projectileOffset, -areaSize.y - projectileOffset),
				new Vector2(areaSize.x + projectileOffset, areaSize.y + projectileOffset));

			if (!IsTestMode)
				CreateGameWorldVisuals(areaSizeType, areaSize, bounds);

			world.SetSingletonInstance(ScoreComp.Default);
			world.SetSingletonInstance<GoldComp>();
			world.SetSingletonInstance<KillStreakComp>();
			world.SetSingletonInstance(UpgradesComp.Default);
			world.SetSingletonInstance(EnemyUpgradesComp.Default);
			world.SetSingletonInstance(new SpawnAreaComp { Value = new SpawnArea(bounds) });
			world.SetSingletonInstance<WaveInfoComp>();
			world.SetSingletonInstance(new DeadEnemiesListComp { Value = new List<DeadEnemiesListComp.DeadEnemyInfo>() });
			world.SetSingletonInstance(new AreaSizeComp { Value = areaSizeType });
			world.SetSingletonInstance(new DifficultyComp { Value = difficulty });
			world.SetSingletonInstance(new ShopUpgradesComp() { Upgrades = new UpgradeType[3], IsBought = new bool[3] });
			var musicSource = new GameObject("MusicSource").AddComponent<AudioSource>();
			musicSource.clip = Resources.Load<AudioClip>("Audio/Music/Main");
			musicSource.loop = true;
			musicSource.volume = 0.16f;
			musicSource.Play();
			world.Set(new AudioComp { Value = new GameObject("EffectSource").AddComponent<AudioSource>() });

			var coreSystem = new CoreSystem<float>(
				new InputSystem(world),
				new MousePositionSystem(world, camera),
				new MouseInputSystem(world),
				new PlayerMovementSystem(world, bounds),
				new AutomaticTargetingSystem(world),
				new WandRotationSystem(world),
				//
				new WaveSystem(world, dataController.WaveData, dataController.GameData),
				new EnemySpawnSystem(world, dataController.EnemyRecipes),
				new EnemyFollowSystem(world),
				new EnemyFollowMapEdgeSystem(world),
				new EnemyFollowDistanceSystem(world),
				new SetInitialDestinationSystem(world, bounds),
				new EnemySetDestinationSystem(world, bounds),
				new EnemyChargeTimeSystem(world),
				new EnemyChargeFollowSystem(world),
				new EnemyGrowSystem(world), //No dependencies
				new EnemyBuffSystem(world),
				new BufferEnemySystem(world),
				//
				//Render, needs to be before flash, if we want to keep prio of flash effects over health display
				new HealthDisplaySystem(world),
				new FlashEffectSystem(world),
				new EnemyAttackedFlashSystem(world),
				//
				new AttackSystem(world),
				new ProjectileSpawnSystem(world),
				new ProjectileHomingSystem(world),
				new EnemyMeleeAttackSystem(world),
				new EnemyRangedAttackSystem(world),
				new EnemyRangedDelayedAttackSystem(world),
				//
				new InitColdSystem(world),
				new ColdSystem(world),
				new PoisonSystem(world),
				new BleedingSystem(world),
				new CreateCircleEffectSystem(world),
				new CircleEffectSystem(world),
				new PlayerCurseSystem(world),
				new InitEnemyPinballBounds(world),
				new EnemyPinballBounds(world, bounds),
				new EnemyBoundsCollisionSystem(world, bounds), //Before velocity
				new VelocitySystem(world),
				new EnemyChargeBoundsCollisionSystem(world, bounds),
				//
				new RuptureSystem(world),
				new ProjectileCollisionSystem(world),
				new BurningSystem(world),
				new ProjectileBoundsSystem(world, bounds),
				new TimeOutSystem(world),
				//
				new RenderingSystem(world),
				new EnemyRenderingSystem(world),
				new PlayerDirectionRenderingSystem(world),
				new ShadowRenderingSystem(world),
				//
				new PhaseSystem(world),
				new InvulnerableSystem(world),
				new UnHittableSystem(world),
				//
				new HealthRegenSystem(world), //Before DamageSystem
				new DamageSystem(world),
				new SleepSystem(world),
				new PiercingSystem(world),
				new PlayerDamageSystem(world),
				new CurseSystem(world),
				new HealthSystem(world, dataController.GameData),
				new HealthBarSystem(world, camera),
				new DeathMarkSystem(world),
				new ExplodeOnDeathSystem(world),
				new SplitSystem(world), //After all AddDestroy systems
				new DestroySystem(world),
				new KillStreakSystem(world), //No dependencies
				//
				new CameraSystem(world, camera, bounds),
				new SpawnAreaSystem(world, camera, bounds),
				//
				new MultiplierSystem(world),
				new ResetInputSystem(world),
				//
				new FrameStateEndSystem(world, dataController.GameData)
			);

			return coreSystem;
		}

		private static void CreateGameWorldVisuals(AreaSizeType areaSizeType, Vector2 areaSize, Bounds bounds)
		{
			var boundsSquare = new GameObject("Bounds");
			var boundsRenderer = boundsSquare.AddComponent<SpriteRenderer>();
			boundsRenderer.sortingOrder = -4;
			boundsRenderer.sharedMaterial = Resources.Load<Material>("Materials/Ground");
			boundsRenderer.sprite = Resources.Load<Sprite>("Textures/Ground");
			boundsRenderer.drawMode = SpriteDrawMode.Tiled;
			boundsRenderer.size = new Vector2(16, 16);
			boundsRenderer.color = new Color(0.8f, 0.8f, 0.8f);
			boundsSquare.transform.localScale = new Vector3(areaSize.x / 7.8125f, areaSize.y / 7.8125f); //64f, 64f);
			//boundsSquare.transform.localScale = new Vector3(bounds.Size.x / spriteRenderer.sprite.bounds.size.x + 2,
			//	bounds.Size.y / spriteRenderer.sprite.bounds.size.y + 2, 1);
			boundsSquare.transform.position = new Vector3(bounds.Center.x, bounds.Center.y, 100);

			GameObject.Find("Background").transform.localScale *= areaSizeType.GetMultiplier();
			foreach (string edgeString in new[] { "RightEdge", "LeftEdge", "TopEdge", "BottomEdge" })
			{
				var edge = GameObject.Find(edgeString);
				edge.transform.position *= areaSizeType.GetPositionMultiplier();
				var edgeRenderer = edge.GetComponent<SpriteRenderer>();
				edgeRenderer.size = new Vector2(edgeRenderer.size.x, edgeRenderer.size.y * areaSizeType.GetMultiplier());
			}

			//Place random rocks in the background
			var rocks = Resources.LoadAll<Sprite>("Textures/Rocks/");
			var rockParent = new GameObject("Rocks");
			for (int i = 0; i < 75 * areaSizeType.GetMultiplier(); i++)
			{
				var rock = new GameObject("Rock");
				rock.transform.parent = rockParent.transform;
				var rockRenderer = rock.AddComponent<SpriteRenderer>();
				rockRenderer.sortingOrder = -2;
				var rockSprite = rocks[Random.Range(0, rocks.Length)];
				rockRenderer.sprite = rockSprite;
				rockRenderer.color = new Color(0.4f, 0.4f, 0.4f);
				rockRenderer.flipX = Random.Range(0, 2) == 0;
				rock.transform.position = new Vector3(Random.Range(-areaSize.x, areaSize.x), Random.Range(-areaSize.y, areaSize.y), 0);
				float scale = Random.Range(1f, 2f);
				rock.transform.localScale = new Vector3(scale, scale, 1);

				var shadow = new GameObject("Shadow");
				shadow.transform.parent = rock.transform;
				shadow.transform.localPosition = new Vector3(0, -0.95f, 0);
				shadow.transform.localScale = new Vector3(1f, 1f, 1f);
				var shadowRenderer = shadow.AddComponent<SpriteRenderer>();
				shadowRenderer.sortingOrder = -3;
				shadowRenderer.sprite = rockSprite;
				shadowRenderer.color = new Color(0f, 0f, 0f, 0.3f);
				shadowRenderer.flipX = rockRenderer.flipX;
			}
		}
	}
}