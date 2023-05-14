namespace RoguelikeEcs.Core
{
	public readonly struct EnemyTimeSpawnData : IEnemySpawnData
	{
		public EnemyType Type { get; }
		public float PercentTime { get; }
		public float[] PercentTimes { get; }

		public int Count => PercentTimes?.Length ?? 1;

		public EnemyTimeSpawnData(EnemyType type, params float[] percentTimes)
		{
			Type = type;
			PercentTimes = percentTimes;
			PercentTime = 0;
		}

		public EnemyTimeSpawnData(EnemyType type, float percentTime)
		{
			Type = type;
			PercentTimes = null;
			PercentTime = percentTime;
		}
	}
}