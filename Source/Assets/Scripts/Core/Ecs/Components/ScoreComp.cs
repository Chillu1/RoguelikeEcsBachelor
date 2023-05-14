using System.Runtime.CompilerServices;
using DefaultEcs;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	public struct ScoreComp
	{
		public int Kills;
		public int BossesKilled;
		public float DamageDealt;
		public float AoEDamage;
		public float DamageTaken;
		public int ProjectilesFired;
		public int EnemiesHitWithProjectiles;
		public int MissedProjectiles;
		public float DistanceTraveled;

		public int HighestKillStreak;
		public float LowestDamage;

		public float TotalScore;

		public int TotalKills;
		public int TotalBossesKilled;
		public float TotalDamageDealt;
		public float TotalDamageTaken;
		public int HighestKillStreakEver;

		public static ScoreComp Default => new ScoreComp
		{
			LowestDamage = float.MaxValue
		};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ResetWave(World world)
		{
			TotalKills += Kills;
			TotalBossesKilled += BossesKilled;
			TotalDamageDealt += DamageDealt;
			TotalDamageTaken += DamageTaken;
			HighestKillStreakEver = Mathf.Max(HighestKillStreakEver, HighestKillStreak);
			world.Publish(new WaveResetEvent(ref this));

			Kills = 0;
			AoEDamage = 0;
			BossesKilled = 0;
			DamageDealt = 0;
			DamageTaken = 0;
			ProjectilesFired = 0;
			EnemiesHitWithProjectiles = 0;
			MissedProjectiles = 0;
			DistanceTraveled = 0;

			HighestKillStreak = 0;
			LowestDamage = float.MaxValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SubmitStreak(int streak)
		{
			if (streak > HighestKillStreak)
				HighestKillStreak = streak;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SubmitLowestDamage(float damage)
		{
			if (LowestDamage > damage)
				LowestDamage = damage;
		}
	}
}