using System;
using System.Collections.Generic;
using DefaultEcs;
using DefaultEcs.System;
using RoguelikeEcs.Core.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RoguelikeEcs.Core
{
	[With(typeof(IsEnemy), typeof(SpawnComp))]
	public sealed class EnemySpawnSystem : CustomEntitySetSystem
	{
		private readonly IReadOnlyDictionary<EnemyType, EnemyPrototype> _enemyPrototypes;

		private readonly GameObject _enemyParent;

		private readonly Material _material;
		private readonly Dictionary<string, Sprite> _enemyTextures;

		public EnemySpawnSystem(World world, EnemyRecipes enemyRecipes) : base(world)
		{
			_enemyPrototypes = enemyRecipes.GetPrototypes();

			_enemyParent = new GameObject("Enemies");

			_material = Resources.Load<Material>("Materials/Material");
			_enemyTextures = new Dictionary<string, Sprite>();
			foreach (var enemyTexture in Resources.LoadAll<Sprite>("Textures"))
				_enemyTextures.Add(enemyTexture.name, enemyTexture);

			world.Subscribe<SpawnEnemyEvent>(SpawnEnemy);
			world.Subscribe<SpawnSplitEnemyEvent>(SpawnSplitEnemy);
			world.Subscribe<LoadEnemiesEvent>(LoadEnemies);
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			var deadEnemies = World.Get<DeadEnemiesListComp>().Value;

			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var enemy = ref entities[i];
				ref var spawn = ref enemy.Get<SpawnComp>();
				spawn.Timer += state;

				if (spawn.Timer < spawn.SpawnInterval)
					continue;

				if (enemy.Has<IsNecromancerEnemy>())
				{
					if (deadEnemies.Count > 0)
					{
						ref readonly var position = ref enemy.Get<PositionComp>();
						for (int j = 0; j < deadEnemies.Count; j++)
						{
							var deadEnemyInfo = deadEnemies[j];
							if (Vector2.Distance(position.Value, deadEnemyInfo.Position) < 100f)
							{
								Spawn(deadEnemyInfo.EnemyType, deadEnemyInfo.Position);
								deadEnemies.RemoveAt(j);
								spawn.Timer = 0;
								break;
							}
						}

						//Reset time partially, so we don't iterate over the whole list every frame
						spawn.Timer = spawn.SpawnInterval * 0.9f;
					}

					continue;
				}

				for (int j = 0; j < spawn.EnemiesCount; j++)
				{
					Vector2 offset = Random.insideUnitCircle * 5f;
					Spawn(spawn.SpawnType, enemy.Get<PositionComp>().Value + offset);
				}

				spawn.Timer = 0;
			}
		}

		private void SpawnEnemy(in SpawnEnemyEvent spawnEnemyEvent)
		{
			if (spawnEnemyEvent.Position == Vector2.zero)
				Spawn(spawnEnemyEvent.Type);
			else
				Spawn(spawnEnemyEvent.Type, spawnEnemyEvent.Position);
		}

		internal Entity Spawn(EnemyType enemyType)
		{
			if (!_enemyPrototypes.TryGetValue(enemyType, out var prototype))
			{
				Debug.LogError($"No prototype for enemy type {enemyType}");
				return default;
			}

			Vector2 spawnPosition;
			ref readonly var spawnArea = ref World.Get<SpawnAreaComp>();
			if (prototype.Original.Has<IsMapEdgeEnemy>())
			{
				spawnPosition = spawnArea.Value.GetRandomPositionOnEdge();
				float offset = spawnPosition.x < 0 ? -1 : 1;
				spawnPosition.x += offset * 25f;
			}
			else if (prototype.Original.Has<SpawnInBoundsComp>())
			{
				var playerPosition = World.GetEntities().With<IsPlayerComp>().AsSet().GetFirst().Get<PositionComp>().Value;
				spawnPosition = spawnArea.Value.GetRandomPositionInBounds(playerPosition);
			}
			else
				spawnPosition = spawnArea.Value.GetRandomPosition();

			return Spawn(prototype, spawnPosition);
		}

		private Entity Spawn(EnemyType enemyType, Vector2 spawnPosition)
		{
			if (!_enemyPrototypes.TryGetValue(enemyType, out var prototype))
			{
				Debug.LogError($"No prototype for enemy type {enemyType}");
				return default;
			}

			return Spawn(prototype, spawnPosition);
		}

		private Entity Spawn(EnemyPrototype prototype, Vector2 position)
		{
			return SpawnEnemy(prototype.Original, position, prototype.Recipe.EnemyType.ToString(), prototype.Recipe.ColorU,
				prototype.Recipe.SortingLayerU, prototype.Recipe.LayerU, prototype.Recipe.ScaleU, prototype.Recipe.MassU);
		}

		private void SpawnSplitEnemy(in SpawnSplitEnemyEvent message)
		{
			const float splitMultiplier = 0.667f;

			ref readonly var original = ref message.Enemy;
			bool wasBoss = original.Has<IsBossEnemy>();
			int splitCount = original.Get<SplitComp>().Count;
			var oldGo = original.Get<GameObjectComp>().Value;
			var position = original.Get<PositionComp>().Value;
			var scale = original.Get<ScaleComp>().Value;
			float mass = original.Get<RigidbodyComp>().Value.mass;

			for (int i = 0; i < splitCount; i++)
			{
				Color color = Color.white;
				string sortingLayerName = "Default";
				if (!Core.IsTestMode)
				{
					ref readonly var oldRenderer = ref original.Get<RendererComp>();
					color = oldRenderer.OriginalColor;
					sortingLayerName = oldRenderer.Value.sortingLayerName;
				}

				Vector2 randomAddedPosition = new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f));

				var entity = SpawnEnemy(original, position + randomAddedPosition, "Split", color, sortingLayerName, oldGo.layer,
					scale * splitMultiplier, mass * splitMultiplier);
				entity.Remove<DestroyComp>();
				if (wasBoss)
					entity.Remove<IsBossEnemy>();

				ref var health = ref entity.Get<HealthComp>();
				health.Current = health.Max = health.Max * splitMultiplier;
				entity.Get<DamageComp>().Value *= splitMultiplier;
				ref var crit = ref entity.Get<CritComp>();
				crit.Chance *= splitMultiplier;
				crit.Multiplier *= splitMultiplier;
				entity.Get<EvasionComp>().Chance *= splitMultiplier; //TODO N isn't reset
				//TODO resistances * splitMultiplier
				entity.Get<VelocityComp>().Speed *= splitMultiplier;
				ref var scaleComp = ref entity.Get<ScaleComp>();
				scaleComp.Value = scale * splitMultiplier;
				scaleComp.Magnitude = scaleComp.Value.GetMagnitude();

				//Makes entity invulnerable for a short time, so it doesn't get hit by the same projectile that killed it
				entity.Set(new InvulnerableComp() { TimeLeft = 0.2f });
			}
		}

		private Entity SpawnEnemy(Entity original, Vector2 position, string name, Color color, string sortingLayerName, int layer,
			Vector2 scale, float mass)
		{
			var entity = original.CopyTo(World);

			entity.Get<PositionComp>().Value = position;
			if (original.Has<IsBossEnemy>())
				entity.Set(HealthBarSystem.CreateNewHealthBar(Mathf.Lerp(0.7f, 3f, scale.x / 30f)));
			entity.Get<DamageEventsComp>().DamageEvents = new List<DamageEvent>(3);

			ref var multiplier = ref entity.Get<MultiplierComp>();
			multiplier.BaseMultipliers = new float[] { 1, 1, 1, 1 };
			multiplier.MultipliersList = new List<MultiplierData>(5);

			var go = new GameObject(name + entity);
			if (!Core.IsTestMode)
			{
				var child = new GameObject("Sprite");
				child.transform.parent = go.transform;
				var renderer = child.AddComponent<SpriteRenderer>();
				renderer.material = _material;

				var enemyType = original.Get<EnemyTypeComp>().Value;
				if ((entity.Has<IsBossEnemy>() || enemyType.ToString().Contains("Boss")) &&
				    !Enum.TryParse(enemyType.ToString().Replace("Boss", ""), out enemyType))
				{
					enemyType = EnemyType.None; //Use default sprite
				}

				_enemyTextures.TryGetValue(enemyType.ToString(), out var sprite);

				if (sprite == null)
					sprite = _enemyTextures["Enemy"];
				renderer.sprite = sprite;
				renderer.color = color;
				renderer.sortingLayerName = sortingLayerName;
				entity.Set(new RendererComp { Value = renderer, Child = child, OriginalColor = color });

				var shadow = new GameObject("Shadow");
				shadow.transform.parent = go.transform;
				shadow.transform.localPosition = new Vector3(0, -0.8f, 0);
				var shadowRenderer = shadow.AddComponent<SpriteRenderer>();
				shadowRenderer.sortingOrder = -1;
				shadowRenderer.material = _material;
				shadowRenderer.sprite = Resources.Load<Sprite>("Textures/Shadow");
				entity.Set(new ShadowRendererComp { Value = shadowRenderer });
			}

			go.layer = layer;
			go.transform.position = new Vector3(position.x, position.y, 0);
			go.transform.localScale = scale;
			//TODO BoxCollider scale will be wrong since we don't have a sprite renderer
			var collider = go.AddComponent<BoxCollider2D>();
			collider.size = new Vector2(8, 8); //TODO Proper scaling
			go.transform.SetParent(_enemyParent.transform);
			var rb = go.AddComponent<Rigidbody2D>();
			rb.freezeRotation = true;
			rb.gravityScale = 0f;
			rb.interpolation = RigidbodyInterpolation2D.Interpolate;
			rb.mass = mass;

			if (name == "Snake")
			{
				var entityBody = World.CreateEntity();

				var snakeGo = new GameObject(name + entity + "snakebody");
				snakeGo.layer = layer;
				snakeGo.transform.position = new Vector3(position.x + 15, position.y + 15, 0);
				snakeGo.transform.localScale = scale;
				snakeGo.AddComponent<BoxCollider2D>();
				var snakeRb = snakeGo.AddComponent<Rigidbody2D>();
				snakeRb.freezeRotation = false;
				snakeRb.gravityScale = 0f;
				snakeRb.interpolation = RigidbodyInterpolation2D.Interpolate;
				snakeRb.mass = mass;
				var snakeRenderer = snakeGo.AddComponent<SpriteRenderer>();
				snakeRenderer.material = _material;
				snakeRenderer.sprite = _enemyTextures["Enemy"];
				snakeRenderer.color = color;
				snakeRenderer.sortingLayerName = sortingLayerName;
				entityBody.Set(new RendererComp { Value = snakeRenderer, OriginalColor = color });

				var hinge = go.AddComponent<HingeJoint2D>();
				hinge.connectedBody = snakeRb;
				hinge.anchor = new Vector2(5, 0);
				hinge.connectedAnchor = new Vector2(-5, 0);
				hinge.useLimits = true;
				hinge.limits = new JointAngleLimits2D() { max = 150, min = 210 };
			}

			var reference = go.AddComponent<EnemyReference>();
			reference.Setup(entity);

			entity.Set(new GameObjectComp() { Value = go });
			entity.Set(new RigidbodyComp() { Value = rb });

			var enemyBuff = new EnemyBuffComp() { Buffs = new Dictionary<BuffType, EnemyBuffComp.Timer>() };
			entity.Set(enemyBuff);

			return entity;
		}

		private void LoadEnemies(in LoadEnemiesEvent message)
		{
			foreach (var enemySpawnData in message.EnemySpawnData)
			{
				var enemy = Spawn(enemySpawnData.EnemyType, enemySpawnData.Position);
				ref var health = ref enemy.Get<HealthComp>();
				health.Current = enemySpawnData.HealthPercent * health.Max;
				if (enemy.Has<SplitComp>())
				{
					enemy.Get<SplitComp>().SplitsLeft = enemySpawnData.SplitCount;
					//Reduce states by how many times it has split
					var prototype = _enemyPrototypes[enemySpawnData.EnemyType];
					int splitAmount = prototype.Original.Get<SplitComp>().SplitsLeft - enemySpawnData.SplitCount;
					ScaleSplitEnemy(in enemy, splitAmount);
				}
			}
		}

		private static void ScaleSplitEnemy(in Entity entity, int splitAmount)
		{
			if (splitAmount <= 0)
				return;

			if (entity.Has<IsBossEnemy>())
				entity.Remove<IsBossEnemy>();

			const float splitMultiplier = 0.667f;
			float splitValue = splitMultiplier * splitAmount;

			ref var health = ref entity.Get<HealthComp>();
			health.Current = health.Max = health.Max * splitValue;
			entity.Get<DamageComp>().Value *= splitValue;
			ref var crit = ref entity.Get<CritComp>();
			crit.Chance *= splitValue;
			crit.Multiplier *= splitValue;
			entity.Get<EvasionComp>().Chance *= splitValue; //TODO N isn't reset
			//TODO resistances * splitMultiplier
			entity.Get<VelocityComp>().Speed *= splitValue;
			ref var scaleComp = ref entity.Get<ScaleComp>();
			scaleComp.Value *= splitValue;
			scaleComp.Magnitude = scaleComp.Value.GetMagnitude();

			entity.Get<GameObjectComp>().Value.transform.localScale *= splitValue;
			entity.Get<RigidbodyComp>().Value.mass *= splitValue;
		}
	}
}