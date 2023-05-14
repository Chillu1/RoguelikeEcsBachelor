namespace RoguelikeEcs.Core
{
	public struct SpawnComp : IRecipeComp
	{
		public EnemyType SpawnType;
		public float SpawnInterval;
		public int EnemiesCount;

		public float Timer;
	}
}