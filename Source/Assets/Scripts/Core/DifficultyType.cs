using System;

namespace RoguelikeEcs.Core
{
	public enum DifficultyType
	{
		Easy = -1,
		Normal = 0,
		Hard = 1,
		Impossible = 2,
	}

	public static class DifficultyTypeExtensions
	{
		public static float GetSpawnIntervalMultiplier(this DifficultyType difficulty)
		{
			return difficulty switch
			{
				DifficultyType.Easy => 1.2f,
				DifficultyType.Normal => 1f,
				DifficultyType.Hard => 0.8f,
				DifficultyType.Impossible => 0.6f,
				_ => throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, null)
			};
		}

		public static float GetPlayerDamageMultiplier(this DifficultyType difficulty)
		{
			switch (difficulty)
			{
				case DifficultyType.Easy:
					return 1.5f;
				case DifficultyType.Normal:
				case DifficultyType.Hard:
				case DifficultyType.Impossible:
					return 1f;
				default:
					throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, null);
			}
		}
	}
}