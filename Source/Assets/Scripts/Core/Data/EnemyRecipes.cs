using System.Collections.Generic;
using DefaultEcs;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	public sealed class EnemyRecipes
	{
		private readonly Dictionary<EnemyType, EnemyPrototype> _enemyPrototypes;

		public EnemyRecipes()
		{
			_enemyPrototypes = new Dictionary<EnemyType, EnemyPrototype>();
			SetupRecipes();

			var scores = new Dictionary<EnemyType, int>();
			foreach (var enemyPrototype in _enemyPrototypes)
				scores[enemyPrototype.Key] = enemyPrototype.Value.Recipe.ScoreU;
			EnemyData.SetupScores(scores);
		}

		public IReadOnlyDictionary<EnemyType, EnemyPrototype> GetPrototypes() => _enemyPrototypes;

		private void SetupRecipes()
		{
			var recipes = new Dictionary<EnemyType, EnemyRecipe>(10);

			EnemyRecipe Add(EnemyType enemyType)
			{
				var recipe = new EnemyRecipe(enemyType);
				recipes.Add(enemyType, recipe);
				return recipe;
			}

			Add(EnemyType.Basic)
				.Health(100f)
				.Speed(180f)
				.Damage(5f)
				.Scale(new Vector2(2f, 2f))
				.Mass(1f)
				.Score(100)
				.Flags(new IsMeleeEnemy())
				.Comps(new AttackComp() { Cooldown = 1f });

			Add(EnemyType.Charger)
				.Health(200f)
				.Speed(180f)
				.Damage(15f)
				.Scale(new Vector2(3f, 3f))
				.Mass(10f)
				.Score(200)
				.Flags(new IsMeleeEnemy(), new IsChargeEnemy())
				.Comps(new AttackComp() { Cooldown = 2f }, new ChargeTimerComp() { PrepareTime = 2.5f, Timer = 2.5f });

			Add(EnemyType.Flying)
				.Health(20f)
				.Speed(185f)
				.Damage(5f)
				.Scale(new Vector2(1.5f, 1.5f))
				.Score(150)
				.Flags(new IsMeleeEnemy(), new IsFlyingEnemy())
				.Comps(new AttackComp() { Cooldown = 1f })
				.Layer("Flying")
				.SortingLayer("Flying");

			Add(EnemyType.Giant)
				.Health(500f)
				.Speed(150f)
				.Damage(30f)
				.Scale(new Vector2(4f, 4f))
				.Mass(100f)
				.Score(300)
				.Flags(new IsMeleeEnemy(), new IsSmartFollowEnemy())
				.Comps(new AttackComp() { Cooldown = 1f })
				.Layer("Giant");

			Add(EnemyType.Ghost)
				.Health(20f)
				.Speed(200f)
				.Damage(15f)
				.Evasion(0.75f)
				.Mass(0.01f)
				.Score(200)
				.Scale(new Vector2(2f, 2f))
				.Flags(new IsMeleeEnemy(), new IsGhostEnemy())
				.Comps(new AttackComp() { Cooldown = 1f },
					new FlashEffectComp { Value = new Color(1f, 0.85f, 0.70f, 0.05f), IntervalMultiplier = 0.5f })
				.Layer("Ghost");

			Add(EnemyType.Armored)
				.Health(100f)
				.Speed(140f)
				.Damage(20f)
				.Scale(new Vector2(3f, 3f))
				.Mass(50f)
				.Score(500)
				.Flags(new IsMeleeEnemy(), new DoDeflectProjectilesComp())
				.Comps(new AttackComp() { Cooldown = 1f });

			Add(EnemyType.Snake)
				.Health(5_000f)
				.Speed(50f)
				.Damage(30f)
				.Scale(new Vector2(9f, 3f))
				.Mass(100f)
				.Score(300)
				.Flags(new IsMeleeEnemy(), new DestinationPositionComp(), new IsSnakeEnemy())
				.Comps(new AttackComp() { Cooldown = 1f })
				.Layer("NoCollision");

			Add(EnemyType.Splitter)
				.Health(200f)
				.Speed(180f)
				.Damage(30f)
				.Scale(new Vector2(4f, 4f))
				.Mass(5f)
				.Score(300)
				.Flags(new IsMeleeEnemy())
				.Comps(new AttackComp() { Cooldown = 1f }, new SplitComp() { SplitsLeft = 1, Count = 2 });

			Add(EnemyType.Exploder)
				.Health(100f)
				.Speed(180f)
				.Damage(35f)
				.Resistances((DamageType.Explosive, 10_000f))
				.Scale(new Vector2(2f, 2f))
				.Mass(2f)
				.Score(200)
				.Flags(new DoExplodeOnDeathComp())
				.Comps(new RadiusComp() { Value = 20f }, new FlashEffectComp { Value = Color.red, IntervalMultiplier = 4f });

			Add(EnemyType.Sleeper)
				.Health(500f)
				.Speed(180f)
				.Damage(35f)
				.Scale(new Vector2(4f, 4f))
				.Mass(500f)
				.Score(300)
				.Flags(new IsMeleeEnemy(), new SpawnInBoundsComp(), new IsSmartFollowEnemy(), new IsSleeping())
				.Comps(new AttackComp() { Cooldown = 1f });

			Add(EnemyType.Phaser)
				.Health(100f)
				.Speed(180f)
				.Damage(15f)
				.Mass(0.01f)
				.Score(400)
				.Scale(new Vector2(3f, 3f))
				.Flags(new IsMeleeEnemy())
				.Comps(new AttackComp() { Cooldown = 1f },
					new FlashEffectComp { Value = new Color(1f, 0.8f, 0.4f, 0.15f), IntervalMultiplier = 0.5f },
					new PhaseComp() { ApplyInterval = 2f, Duration = 1f, Timer = 1f })
				.Layer("Ghost");

			Add(EnemyType.FireResistant)
				.Health(100f)
				.Speed(180f)
				.Damage(5f)
				.Resistances((DamageType.Fire, 10_000f), (DamageType.Cold, -10_000f))
				.Scale(new Vector2(2f, 2f))
				.Mass(1f)
				.Score(100)
				.Flags(new IsMeleeEnemy())
				.Comps(new AttackComp() { Cooldown = 1f });

			Add(EnemyType.ExplosiveResistant)
				.Health(100f)
				.Speed(180f)
				.Damage(5f)
				.Resistances((DamageType.Explosive, 10_000f))
				.Scale(new Vector2(2f, 2f))
				.Mass(1f)
				.Score(100)
				.Flags(new IsMeleeEnemy())
				.Comps(new AttackComp() { Cooldown = 1f });

			Add(EnemyType.Grabber)
				.Health(2000f)
				.Speed(100f)
				.Damage(30f)
				.Scale(new Vector2(12f, 12f))
				.Mass(100f)
				.Score(500)
				.Flags(new IsRangedEnemy(), new IsMapEdgeEnemy())
				.Comps(new AttackComp() { Cooldown = 1f }, new RangeComp { Value = 120f })
				.Layer("NoCollision"); //TODO Maybe collide only with other grabbers? And freeze RB X Axis

			Add(EnemyType.Nest)
				.Health(500f)
				.Resistances((DamageType.Fire, -1000f))
				.Scale(new Vector2(10f, 10f))
				.Score(300)
				.Texture("Nest")
				.Flags(new SpawnInBoundsComp())
				.Comps(new SpawnComp { SpawnType = EnemyType.Basic, SpawnInterval = 6f, EnemiesCount = 6 })
				.Layer("NoCollision");

			Add(EnemyType.Queen)
				.Health(1000f)
				.Speed(120f)
				.Scale(new Vector2(8f, 8f))
				.Score(500)
				.Texture("Queen")
				.Flags(new SpawnInBoundsComp(), new IsDistanceFollowEnemy())
				.Comps(new SpawnComp { SpawnType = EnemyType.Flying, SpawnInterval = 4f, EnemiesCount = 8 }, new DistanceComp
				{
					Min = 240f, Max = 280f
				});

			Add(EnemyType.Pinball)
				.Health(2000f)
				.Speed(380f)
				.Damage(50f)
				.Resistances((DamageType.Physical, 200f))
				.Scale(new Vector2(6f, 6f))
				.Score(1000)
				.Flags(new IsMeleeEnemy(), new SpawnInBoundsComp(), new IsPinballEnemy())
				.Comps(new AttackComp() { Cooldown = 1f })
				.Layer("NoCollision");

			Add(EnemyType.ExploderPinball)
				.Health(2000f)
				.Speed(380f)
				.Damage(40f)
				.Resistances((DamageType.Physical, 100f), (DamageType.Explosive, 1000f))
				.Scale(new Vector2(6f, 6f))
				.Score(1000)
				.Flags(new IsMeleeEnemy(), new DoExplodeOnDeathComp(), new SpawnInBoundsComp(), new IsPinballEnemy())
				.Comps(new AttackComp() { Cooldown = 1f }, new RadiusComp() { Value = 40f },
					new FlashEffectComp { Value = Color.red, IntervalMultiplier = 4f })
				.Layer("NoCollision");

			Add(EnemyType.Grower)
				.Health(500f)
				.Scale(new Vector2(6f, 6f))
				.Score(500)
				.Texture("Grower")
				.Flags(new SpawnInBoundsComp())
				.Comps(new GrowComp { Time = 10f, EnemyType = EnemyType.BasicBoss })
				.Layer("NoCollision");

			Add(EnemyType.Necromancer)
				.Health(300f)
				.Speed(120f)
				.Scale(new Vector2(7f, 7f))
				.Score(300)
				.Flags(new SpawnInBoundsComp(), new IsDistanceFollowEnemy(), new IsNecromancerEnemy())
				.Comps(new DistanceComp { Min = 240f, Max = 280f }, new SpawnComp { SpawnInterval = 10f })
				.Layer("NoCollision");

			Add(EnemyType.Buffer)
				.Health(300f)
				.Speed(120f)
				.Scale(new Vector2(4f, 4f))
				.Score(300)
				.Flags(new SpawnInBoundsComp(), new IsDistanceFollowEnemy(), new IsBufferEnemy())
				.Comps(new DistanceComp { Min = 120f, Max = 180f }, new RadiusComp() { Value = 100f },
					new BufferEnemyComp() { Interval = 4f, BuffLength = 10f });

			//---Bosses---

			Add(EnemyType.BasicBoss)
				.Health(2500f)
				.Speed(180f)
				.Damage(60f)
				.Scale(new Vector2(8f, 8f))
				.Mass(1000f)
				.Score(3000)
				.Flags(new IsMeleeEnemy(), new IsSmartFollowEnemy(), new IsBossEnemy())
				.Comps(new AttackComp() { Cooldown = 1f })
				.Layer("Giant");

			Add(EnemyType.ChargerBoss)
				.Health(5000f)
				.Speed(180f)
				.Damage(100f)
				.Scale(new Vector2(10f, 10f))
				.Mass(1500f)
				.Score(4000)
				.Flags(new IsMeleeEnemy(), new IsChargeEnemy(), new IsBossEnemy())
				.Comps(new AttackComp() { Cooldown = 2f }, new ChargeTimerComp() { PrepareTime = 2.5f, Timer = 2.5f })
				.Layer("Giant");

			Add(EnemyType.BigBoss)
				.Health(10000f)
				.Speed(150f)
				.Damage(200f)
				.Scale(new Vector2(20f, 20f))
				.Mass(5000f)
				.Score(10000)
				.Flags(new IsMeleeEnemy(), new IsSmartFollowEnemy(), new IsBossEnemy())
				.Comps(new AttackComp() { Cooldown = 1f })
				.Layer("Giant");

			Add(EnemyType.GiganticBoss)
				.Health(50_000f)
				.Speed(150f)
				.Damage(500f)
				.Scale(new Vector2(30f, 30f))
				.Mass(5_000f)
				.Score(50_000)
				.Flags(new IsMeleeEnemy(), new IsSmartFollowEnemy(), new IsBossEnemy())
				.Comps(new AttackComp() { Cooldown = 1f })
				.Layer("Giant");

			Add(EnemyType.SplitterBoss)
				.Health(5000f)
				.Speed(165f)
				.Damage(200f)
				.Scale(new Vector2(20f, 20f))
				.Mass(2500f)
				.Score(5000)
				.Flags(new IsMeleeEnemy(), new IsBossEnemy())
				.Comps(new AttackComp() { Cooldown = 1f }, new SplitComp() { SplitsLeft = 3, Count = 3 });

			Add(EnemyType.PhaserBoss)
				.Health(200_000f)
				.Speed(180f)
				.Damage(800f)
				.Scale(new Vector2(35f, 35f))
				.Mass(20_000f)
				.Score(100_000)
				.Flags(new IsMeleeEnemy(), new IsBossEnemy())
				.Comps(new AttackComp() { Cooldown = 1f }, new SplitComp() { SplitsLeft = 2, Count = 2 },
					new PhaseComp() { ApplyInterval = 2f, Duration = 1f, Timer = 1f })
				.Layer("Giant");

			Add(EnemyType.ResistantBoss)
				.Health(10000f)
				.Speed(150f)
				.Damage(200f)
				.Resistances((DamageType.Physical, 1000f), (DamageType.Fire, 1000f), (DamageType.Cold, 1000f),
					(DamageType.Explosive, 1000f))
				.Scale(new Vector2(20f, 20f))
				.Mass(5000f)
				.Score(20000)
				.Flags(new IsMeleeEnemy(), new IsBossEnemy())
				.Comps(new AttackComp() { Cooldown = 1f })
				.Layer("Giant");

			Add(EnemyType.GhostBoss)
				.Health(5000f)
				.Speed(200f)
				.Damage(50f)
				.Evasion(0.5f)
				.Mass(0.1f)
				.Score(20000)
				.Scale(new Vector2(15f, 15f))
				.Flags(new IsMeleeEnemy(), new IsBossEnemy(), new IsGhostEnemy())
				.Comps(new AttackComp() { Cooldown = 1f },
					new FlashEffectComp { Value = new Color(1f, 0.85f, 0.70f, 0.05f), IntervalMultiplier = 0.5f })
				.Layer("Ghost");

			var prototypeWorld = new World(1_000);
			foreach (var recipe in recipes)
				_enemyPrototypes.Add(recipe.Key, new EnemyPrototype(recipe.Value.CreatePrototype(prototypeWorld), recipe.Value));
		}
	}
}