using System.Collections.Generic;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	public sealed class WaveData
	{
		private readonly List<WaveInfo> _waves;

		public WaveData()
		{
			_waves = new List<WaveInfo>();
			SetupWaves();
		}

		public IReadOnlyList<WaveInfo> GetWaves() => _waves;

		private void SetupWaves()
		{
			void Add(WaveInfo wave) => _waves.Add(wave);

			Add(new WaveInfo(
				maxEnemyScore: 6_000,
				spawnInterval: 0.6f,
				new EnemySpawnData(EnemyType.Basic, 1_000),
				new EnemySpawnData(EnemyType.Charger, 100)
			));

			Add(new WaveInfo(
				maxEnemyScore: 7_000,
				spawnInterval: 0.6f,
				new EnemySpawnData(EnemyType.Basic, 600),
				new EnemySpawnData(EnemyType.Charger, 70),
				new EnemySpawnData(EnemyType.Flying, 400)
			));

			Add(new WaveInfo(
				maxEnemyScore: 8_000,
				spawnInterval: 0.6f,
				new EnemySpawnData(EnemyType.Basic, 500),
				new EnemySpawnData(EnemyType.Charger, 70),
				new EnemySpawnData(EnemyType.Flying, 300),
				new EnemySpawnData(EnemyType.Giant, 100),
				new EnemyTimeSpawnData(EnemyType.Giant, 0.5f)
			));

			Add(new WaveInfo(
				maxEnemyScore: 9_000,
				spawnInterval: 0.55f,
				new EnemySpawnData(EnemyType.Basic, 400),
				new EnemySpawnData(EnemyType.Charger, 70),
				new EnemySpawnData(EnemyType.Flying, 300),
				new EnemySpawnData(EnemyType.Giant, 100),
				new EnemySpawnData(EnemyType.Ghost, 300)
			));

			//Wave 5
			//1st Boss wave. Spawns 1 boss at 25 seconds.
			Add(new WaveInfo(
				maxEnemyScore: 9_000,
				spawnInterval: 0.55f,
				new EnemySpawnData(EnemyType.Basic, 400),
				new EnemyTimeSpawnData(EnemyType.BasicBoss, 0.833f)
			));

			Add(new WaveInfo(
				maxEnemyScore: 10_000,
				spawnInterval: 0.55f,
				new EnemySpawnData(EnemyType.Basic, 200),
				new EnemySpawnData(EnemyType.Charger, 70),
				new EnemySpawnData(EnemyType.Flying, 200),
				new EnemySpawnData(EnemyType.Giant, 100),
				new EnemySpawnData(EnemyType.Ghost, 200),
				new EnemyTimeSpawnData(EnemyType.Armored, 0.333f, 0.5f, 0.833f)
			));

			Add(new WaveInfo(
				maxEnemyScore: 12_000,
				spawnInterval: 0.55f,
				new EnemySpawnData(EnemyType.Basic, 150),
				new EnemySpawnData(EnemyType.Charger, 70),
				new EnemySpawnData(EnemyType.Flying, 150),
				new EnemySpawnData(EnemyType.Giant, 100),
				new EnemySpawnData(EnemyType.Ghost, 150),
				new EnemySpawnData(EnemyType.Armored, 30),
				new EnemySpawnData(EnemyType.Splitter, 120)
			));

			Add(new WaveInfo(
				maxEnemyScore: 12_000,
				spawnInterval: 0.55f,
				new EnemySpawnData(EnemyType.Basic, 100),
				new EnemySpawnData(EnemyType.Charger, 50),
				new EnemySpawnData(EnemyType.Flying, 100),
				new EnemySpawnData(EnemyType.Giant, 80),
				new EnemySpawnData(EnemyType.Ghost, 100),
				new EnemySpawnData(EnemyType.Armored, 20),
				new EnemySpawnData(EnemyType.Splitter, 50),
				new EnemySpawnData(EnemyType.Exploder, 100)
			));

			Add(new WaveInfo(
				maxEnemyScore: 12_000,
				spawnInterval: 0.25f,
				new EnemySpawnData(EnemyType.Basic, 500),
				new EnemySpawnData(EnemyType.Charger, 20)
			));

			//Wave 10
			Add(new WaveInfo( //MOVE ME
				maxEnemyScore: 18_000,
				spawnInterval: 0.15f,
				new EnemySpawnData(EnemyType.Flying, 500),
				new EnemyTimeSpawnData(EnemyType.Armored, 0.333f, 0.5f, 0.833f)
			));

			Add(new WaveInfo(
				maxEnemyScore: 13_000,
				spawnInterval: 0.55f,
				new EnemySpawnData(EnemyType.Basic, 100),
				new EnemySpawnData(EnemyType.Charger, 50),
				new EnemySpawnData(EnemyType.Flying, 100),
				new EnemySpawnData(EnemyType.Giant, 80),
				new EnemySpawnData(EnemyType.Ghost, 100),
				new EnemySpawnData(EnemyType.Armored, 20),
				new EnemySpawnData(EnemyType.Splitter, 50),
				new EnemySpawnData(EnemyType.Exploder, 70),
				new EnemySpawnData(EnemyType.Sleeper, 100)
			));

			Add(new WaveInfo( //MOVE ME
				maxEnemyScore: 16_000,
				spawnInterval: 0.75f,
				new EnemySpawnData(EnemyType.Basic, 500),
				new EnemyTimeSpawnData(EnemyType.Grower, 0.333f, 0.5f, 0.833f)
			));

			Add(new WaveInfo( //MOVE ME
				maxEnemyScore: 13_000,
				spawnInterval: 0.5f,
				new EnemySpawnData(EnemyType.Charger, 30),
				new EnemySpawnData(EnemyType.Exploder, 300)
			));

			Add(new WaveInfo( //MOVE ME
				maxEnemyScore: 13_000,
				spawnInterval: 0.4f,
				new EnemySpawnData(EnemyType.Flying, 200),
				new EnemySpawnData(EnemyType.Ghost, 200),
				new EnemySpawnData(EnemyType.Phaser, 50)
			));

			//Wave 15
			Add(new WaveInfo( //MOVE ME
				maxEnemyScore: 16_000,
				spawnInterval: 0.25f,
				new EnemySpawnData(EnemyType.Basic, 200),
				new EnemyTimeSpawnData(EnemyType.Nest, 0.333f, 0.5f, 0.833f)
			));

			Add(new WaveInfo( //MOVE ME
				maxEnemyScore: 17_000,
				spawnInterval: 0.2f,
				new EnemySpawnData(EnemyType.Flying, 200),
				new EnemyTimeSpawnData(EnemyType.Queen, 0.333f, 0.5f, 0.833f)
			));

			Add(new WaveInfo( //MOVE ME
				maxEnemyScore: 16_000,
				spawnInterval: 0.5f,
				new EnemySpawnData(EnemyType.Basic, 500),
				new EnemySpawnData(EnemyType.Giant, 200),
				new EnemySpawnData(EnemyType.Pinball, 70),
				new EnemySpawnData(EnemyType.ExploderPinball, 20)
			));

			Add(new WaveInfo( //MOVE ME
				maxEnemyScore: 16_000,
				spawnInterval: 0.5f,
				new EnemySpawnData(EnemyType.Basic, 300),
				new EnemySpawnData(EnemyType.Charger, 150),
				new EnemySpawnData(EnemyType.Ghost, 150),
				new EnemyTimeSpawnData(EnemyType.Necromancer, 0.333f, 0.5f, 0.833f)
			));

			//Wave ideas:
			// - Tank wave: armored, giants, chargers
			//Missing enemy introduction waves: Buffer, most bosses

			//Splitter boss wave
			Add(new WaveInfo(
				maxEnemyScore: 18_000,
				spawnInterval: 0.55f,
				new EnemySpawnData(EnemyType.Splitter, 200),
				new EnemyTimeSpawnData(EnemyType.SplitterBoss, 0.833f)
			));

			//Unbalanced territory
			//TEMP Automatic waves for testing
			for (int i = 0; i < 250; i++)
			{
				Add(new WaveInfo(
					maxEnemyScore: 20_000 + i * 13_000,
					spawnInterval: Mathf.Max(0.01f, 0.4f - i * 0.002334f),
					new EnemySpawnData(EnemyType.Basic, 100),
					new EnemySpawnData(EnemyType.Charger, 100 + i * 1),
					new EnemySpawnData(EnemyType.Flying, 100 + i * 1),
					new EnemySpawnData(EnemyType.Giant, 75 + i * 1),
					new EnemySpawnData(EnemyType.Ghost, 100 + i * 1),
					new EnemySpawnData(EnemyType.Armored, 40 + i * 1),
					//new EnemySpawnData(EnemyType.Snake, 10 + i * 2),
					new EnemySpawnData(EnemyType.Splitter, 20 + i * 1),
					new EnemySpawnData(EnemyType.Exploder, 20 + i * 1),
					new EnemySpawnData(EnemyType.Sleeper, 20 + i * 1),
					new EnemySpawnData(EnemyType.Phaser, 20 + i * 1),
					new EnemySpawnData(EnemyType.FireResistant, 20 + i * 2),
					new EnemySpawnData(EnemyType.ExplosiveResistant, 20 + i * 1),
					new EnemySpawnData(EnemyType.Grabber, (int)(10 + i * 0.5f)),
					new EnemySpawnData(EnemyType.Nest, (int)(10 + i * 1f)),
					new EnemySpawnData(EnemyType.Queen, (int)(10 + i * 1f)),
					new EnemySpawnData(EnemyType.Pinball, (int)(10 + i * 1f)),
					new EnemySpawnData(EnemyType.ExploderPinball, (int)(10 + i * 1f)),
					new EnemySpawnData(EnemyType.Grower, (int)(10 + i * 1f)),
					new EnemySpawnData(EnemyType.BasicBoss, (int)(10 + i * 1f)),
					new EnemySpawnData(EnemyType.ChargerBoss, (int)(4 + i * 1f)),
					new EnemySpawnData(EnemyType.BigBoss, (int)(2 + i * 1f)),
					new EnemySpawnData(EnemyType.GiganticBoss, (int)(1 + i * 0.5f)),
					new EnemySpawnData(EnemyType.SplitterBoss, (int)(1 + i * 0.5f)),
					new EnemySpawnData(EnemyType.ResistantBoss, (int)(1 + i * 0.5f)),
					new EnemySpawnData(EnemyType.GhostBoss, (int)(1 + i * 0.5f))
				));
			}
		}
	}
}