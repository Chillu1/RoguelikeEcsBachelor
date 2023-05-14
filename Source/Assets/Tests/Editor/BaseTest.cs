using System;
using System.Linq;
using DefaultEcs;
using DefaultEcs.System;
using NUnit.Framework;
using RoguelikeEcs.Core;

namespace RoguelikeEcs.Tests
{
	[Flags]
	public enum SystemType
	{
		None,
		Destroy = 0x1,
		Health = 0x2,
		Wave = 0x4,
	}

	public abstract class BaseTest
	{
		protected World World;
		protected CoreSystem<float> System;
		protected EnemySpawnSystem EnemySpawnSystem;
		protected ProjectileSpawnSystem ProjectileSpawnSystem;
		protected ProjectileCollisionSystem ProjectileCollisionSystem;

		protected const float DeltaTime = 0.016666667f;

		protected Entity Player;
		protected Entity Enemy;

		protected float PlayerHealth = 100;
		//protected float PlayerDamage = 2;

		protected float EnemyHealth;
		protected float EnemyDamage;

		[SetUp]
		public void SetUp()
		{
			World = new World();
			System = Core.Core.GetGameLoopSystem(World, AreaSizeType.Medium, DifficultyType.Normal, new DataController(), testMode: true);
			EnemySpawnSystem = System.GetSystem<EnemySpawnSystem>();
			ProjectileSpawnSystem = System.GetSystem<ProjectileSpawnSystem>();
			ProjectileCollisionSystem = System.GetSystem<ProjectileCollisionSystem>();
			var characterData = new CharacterData();

			Player = PlayerSpawner.Spawn(World, characterData.GetCharacterInfo(CharacterType.Default), 0, 0, PlayerHealth);

			Enemy = EnemySpawnSystem.Spawn(EnemyType.Basic);
			EnemyHealth = Enemy.Get<HealthComp>().Max;
			EnemyDamage = Enemy.Get<DamageComp>().Value;

			UpdateSystems();
		}

		[TearDown]
		public void TearDown()
		{
			World.Dispose();
		}

		protected void DisableSystems(params SystemType[] types)
		{
			foreach (var type in types)
			{
				//If type is not a power of 2. Decompose it. Ignore 0/None
				if ((type & (type - 1)) != 0)
				{
					DisableSystems(Enum.GetValues(type.GetType()).Cast<SystemType>().Where(t => t != SystemType.None).ToArray());
					continue;
				}

				switch (type)
				{
					case SystemType.Destroy:
						System.GetSystem<DestroySystem>().IsEnabled = false;
						break;
					case SystemType.Health:
						System.GetSystem<HealthSystem>().IsEnabled = false;
						break;
					case SystemType.Wave:
						System.GetSystem<WaveSystem>().IsEnabled = false;
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(type), type, null);
				}
			}
		}

		protected Entity SpawnProjectile()
		{
			return ProjectileSpawnSystem.SpawnProjectile(Player.Get<PositionComp>().Value,
				Player.Get<PlayerMouseClickComp>().Position,
				ref Player.Get<AttackDataComp>());
		}

		protected void UpdateSystems(float delta = 0f) => System.Update(delta);
	}
}