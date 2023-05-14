using DefaultEcs;
using RoguelikeEcs.Core.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RoguelikeEcs.Core
{
	public sealed class EnemyRecipe
	{
		public EnemyType EnemyType { get; }

		//TODO Terrible name conflict fix
		public Vector2 ScaleU { get; private set; }
		public float MassU { get; private set; }
		public int ScoreU { get; private set; }
		public Color ColorU { get; private set; }
		public string TextureU { get; private set; }
		public int LayerU { get; private set; }
		public string SortingLayerU { get; private set; }

		private float _health, _speed, _damage, _critChance, _critMultiplier, _evasion;
		private (DamageType Type, float Value)[] _resistancesU;

		private IFlagComp[] _flags;
		private IRecipeComp[] _recipeComps;

		public EnemyRecipe(EnemyType enemyType)
		{
			EnemyType = enemyType;
			_critChance = 0.0f;
			_critMultiplier = 2f;
			ScaleU = new Vector2(2f, 2f);
			MassU = 1f;
			ColorU = UnityEngine.Color.white;
			TextureU = "Enemy";
			_flags = null;
			_recipeComps = null;

			LayerU = LayerMask.NameToLayer("Default");
			SortingLayerU = "Default";
		}

		public EnemyRecipe Health(float health)
		{
			_health = health;
			return this;
		}

		public EnemyRecipe Speed(float speed)
		{
			_speed = speed;
			return this;
		}

		public EnemyRecipe Damage(float damage)
		{
			_damage = damage;
			return this;
		}

		public EnemyRecipe Resistances(params (DamageType Type, float Value)[] resistances)
		{
			_resistancesU = resistances;
			return this;
		}

		public EnemyRecipe Crit(float chance, float multiplier)
		{
			_critChance = chance;
			_critMultiplier = multiplier;
			return this;
		}

		public EnemyRecipe Evasion(float evasion)
		{
			if (evasion > 1f)
				evasion *= 0.01f;
			_evasion = evasion;
			return this;
		}

		public EnemyRecipe Scale(Vector2 scale)
		{
			ScaleU = scale;
			return this;
		}

		public EnemyRecipe Mass(float mass)
		{
			MassU = mass;
			return this;
		}

		public EnemyRecipe Score(int score)
		{
			ScoreU = score;
			return this;
		}

		public EnemyRecipe Color(Color color)
		{
			ColorU = color;
			return this;
		}

		//Not implemented yet
		public EnemyRecipe Texture(string textureName)
		{
			TextureU = textureName;
			return this;
		}

		public EnemyRecipe Flags(params IFlagComp[] flags)
		{
			_flags = flags;
			return this;
		}

		public EnemyRecipe Comps(params IRecipeComp[] comps)
		{
			_recipeComps = comps;
			return this;
		}

		public EnemyRecipe Layer(string layerName)
		{
			LayerU = LayerMask.NameToLayer(layerName);
			return this;
		}

		public EnemyRecipe SortingLayer(string sortingLayer)
		{
			SortingLayerU = sortingLayer;
			return this;
		}

		public Entity CreatePrototype(World prototypeWorld)
		{
			var entity = prototypeWorld.CreateEntity();

			entity.Set<IsEnemy>();
			entity.Set<DamageEventsComp>();
			entity.Set<MultiplierComp>();

			float speedVariation = 20f;

			for (int i = 0; i < _flags?.Length; i++)
			{
				switch (_flags[i])
				{
					case IsMeleeEnemy comp:
						entity.Set(comp);
						break;
					case IsChargeEnemy comp:
						entity.Set(comp);
						break;
					case DestinationPositionComp comp:
						entity.Set(comp);
						break;
					case IsFlyingEnemy comp:
						entity.Set(comp);
						break;
					case IsGhostEnemy comp:
						entity.Set(comp);
						break;
					case DoDeflectProjectilesComp comp:
						entity.Set(comp);
						break;
					case IsBossEnemy comp:
						entity.Set(comp);
						break;
					case IsSnakeEnemy comp:
						entity.Set(comp);
						break;
					case DoExplodeOnDeathComp comp:
						entity.Set(comp);
						break;
					case IsSleeping comp:
						entity.Set(comp);
						break;
					case IsRangedEnemy comp:
						entity.Set(comp);
						break;
					case IsMapEdgeEnemy comp:
						entity.Set(comp);
						break;
					case SpawnInBoundsComp comp:
						entity.Set(comp);
						break;
					case IsDistanceFollowEnemy comp:
						entity.Set(comp);
						break;
					case IsPinballEnemy comp:
						entity.Set(comp);
						break;
					case IsNecromancerEnemy comp:
						entity.Set(comp);
						break;
					case IsBufferEnemy comp:
						entity.Set(comp);
						break;
					case IsSmartFollowEnemy comp:
						entity.Set(comp);
						break;
					default:
						Debug.LogError($"Unknown flag {_flags[i]}");
						break;
				}
			}

			for (int i = 0; i < _recipeComps?.Length; i++)
			{
				var recipeComp = _recipeComps[i];
				switch (recipeComp) //TODO Not ideal, default supports interfaces
				{
					case AttackComp comp:
						entity.Set(comp);
						break;
					case ChargeTimerComp comp:
						entity.Set(comp);
						break;
					case SplitComp comp:
						entity.Set(comp);
						break;
					case RadiusComp comp:
						entity.Set(comp);
						break;
					case FlashEffectComp comp:
						entity.Set(comp);
						break;
					case PhaseComp comp:
						entity.Set(comp);
						break;
					case RangeComp comp:
						entity.Set(comp);
						break;
					case SpawnComp comp:
						entity.Set(comp);
						break;
					case DistanceComp comp:
						entity.Set(comp);
						break;
					case GrowComp comp:
						entity.Set(comp);
						break;
					case BufferEnemyComp comp:
						entity.Set(comp);
						break;
					default:
						Debug.LogError($"Unknown component type {recipeComp.GetType()}");
						break;
				}
			}

			entity.Set(new EnemyTypeComp() { Value = EnemyType });
			entity.Set(new HealthComp() { Current = _health, Max = _health });
			if (_damage > 0)
				entity.Set(new DamageComp() { Value = _damage });
			entity.Set(new CritComp { Chance = _critChance, Multiplier = _critMultiplier });
			entity.Set(new EvasionComp { Chance = _evasion });

			var resistances = ResistancesComp.Default;
			foreach (var tuple in _resistancesU.IsNotNull())
				resistances.AddValue(tuple.Type, tuple.Value);
			entity.Set(resistances);

			if (_speed > 0)
				entity.Set(new VelocityComp { Speed = _speed + Random.Range(-speedVariation, speedVariation) });
			entity.Set(new ScaleComp { Value = ScaleU, Magnitude = ScaleU.GetMagnitude() });

			//Components that need to be filled:
			entity.Set<PositionComp>();
			entity.Set<GameObjectComp>();
			entity.Set<RendererComp>();
			entity.Set<RigidbodyComp>();

			return entity;
		}
	}
}