using System;
using System.Collections.Generic;
using DefaultEcs;

namespace RoguelikeEcs.Core
{
	public sealed class GameData
	{
		public HashSet<UpgradeType> UnlockedUpgrades { get; }
		public HashSet<string> CompletedAchievements { get; }
		public HashSet<CharacterType> UnlockedCharacters { get; }

		public float TotalPlayTime { get; private set; }
		public int TotalKills { get; private set; }
		public int TotalBossKills { get; private set; }
		public float TotalDamage { get; private set; }
		public float TotalAoEDamage { get; private set; }
		public int HighestKillStreak { get; private set; }
		public int ProjectilesFired { get; private set; }
		public int MissedProjectiles { get; private set; }
		public float DistanceTraveled { get; private set; }
		public int WavesCompleted { get; private set; }

		private IDisposable _worldSubscription;

		public GameData()
		{
			UnlockedUpgrades = new HashSet<UpgradeType>();
			CompletedAchievements = new HashSet<string>();
			UnlockedCharacters = new HashSet<CharacterType>();
		}

		public void Subscribe(World world)
		{
			_worldSubscription = world.Subscribe((in WaveResetEvent message) =>
			{
				AddKills(message.Score.Kills);
				AddBossKills(message.Score.BossesKilled);
				AddDamage(message.Score.DamageDealt);
				AddAoEDamage(message.Score.AoEDamage);
				AddKillStreak(message.Score.HighestKillStreak);
				AddProjectilesFired(message.Score.ProjectilesFired);
				AddMissedProjectiles(message.Score.MissedProjectiles);
				AddDistanceTraveled(message.Score.DistanceTraveled);
			});
		}

		public void Unsubscribe() => _worldSubscription?.Dispose();

		public void AddPlayTime(float time) => TotalPlayTime += time;
		public void AddKills(int kills) => TotalKills += kills;
		public void AddBossKills(int kills) => TotalBossKills += kills;
		public void AddDamage(float damage) => TotalDamage += damage;
		public void AddAoEDamage(float damage) => TotalAoEDamage += damage;

		public void AddKillStreak(int streak)
		{
			if (streak > HighestKillStreak)
				HighestKillStreak = streak;
		}

		public void AddProjectilesFired(int projectiles) => ProjectilesFired += projectiles;
		public void AddMissedProjectiles(int projectiles) => MissedProjectiles += projectiles;
		public void AddDistanceTraveled(float distance) => DistanceTraveled += distance;

		public void AddWave() => WavesCompleted++;
	}
}