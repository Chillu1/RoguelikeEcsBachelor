using DefaultEcs;

namespace RoguelikeEcs.Core
{
	public readonly struct SpawnSplitEnemyEvent
	{
		public readonly Entity Enemy;

		public SpawnSplitEnemyEvent(Entity enemy) => Enemy = enemy;
	}
}