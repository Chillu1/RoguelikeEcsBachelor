namespace RoguelikeEcs.Core
{
	public readonly struct EnemySpawnData : IEnemySpawnData
	{
		public EnemyType Type { get; }
		public int SpawnWeight { get; }

		public EnemySpawnData(EnemyType type, int spawnWeight)
		{
			Type = type;
			SpawnWeight = spawnWeight;
		}
	}
}