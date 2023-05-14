using System.Collections.Generic;
using UnityEngine;
using Weighted_Randomizer;

namespace RoguelikeEcs.Core
{
	public sealed class WaveInfo //TODO Rename
	{
		public int MaxEnemyScore { get; }
		public int MaxEnemyScoreInterval { get; }
		public float SpawnInterval { get; }
		public float TotalSpawnWeight => _randomizer.TotalWeight;
		public IReadOnlyList<IEnemySpawnData> EnemySpawnData { get; }

		private readonly StaticWeightedRandomizer<EnemyType> _randomizer;
		private readonly List<EnemyTimeSpawnData> _timedSpawns;

		public WaveInfo(int maxEnemyScore, float spawnInterval, params IEnemySpawnData[] enemySpawnData)
		{
			MaxEnemyScore = maxEnemyScore;
			MaxEnemyScoreInterval = (int)(maxEnemyScore / 6f); //5s intervals
			SpawnInterval = spawnInterval;
			_randomizer = new StaticWeightedRandomizer<EnemyType>();
			_timedSpawns = new List<EnemyTimeSpawnData>();
			EnemySpawnData = enemySpawnData;
			foreach (var spawnData in enemySpawnData)
			{
				switch (spawnData)
				{
					case EnemySpawnData data:
						_randomizer.Add(data.Type, data.SpawnWeight);
						break;
					case EnemyTimeSpawnData data:
						if (data.PercentTimes is { Length: > 0 })
							foreach (float times in data.PercentTimes)
								_timedSpawns.Add(new EnemyTimeSpawnData(data.Type, times));
						else
							_timedSpawns.Add(data);

						break;
					default:
						Debug.LogError($"Unknown spawn data type: {spawnData.GetType()}");
						break;
				}
			}
		}

		public EnemyType GetRandomEnemy(float waveTimePercent)
		{
			if (_timedSpawns.Count > 0)
			{
				EnemyTimeSpawnData data = default;
				foreach (var timedSpawn in _timedSpawns)
				{
					//Debug.Log($"Checking {timedSpawn.Type} at {timedSpawn.PercentTime}. Time: {waveTimePercent}");
					if (waveTimePercent <= timedSpawn.PercentTime)
					{
						data = timedSpawn;
						break;
					}
				}

				if (data.Type != EnemyType.None)
				{
					//TODO More than one possible spawn?
					_timedSpawns.Remove(data);
					return data.Type;
				}
			}

			return _randomizer.NextWithReplacement();
		}
	}
}