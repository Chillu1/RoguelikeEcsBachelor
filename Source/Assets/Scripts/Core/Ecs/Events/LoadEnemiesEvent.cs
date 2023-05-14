using System.Collections.Generic;

namespace RoguelikeEcs.Core
{
	public readonly struct LoadEnemiesEvent
	{
		public readonly List<SaveStateController.EnemyLoadData> EnemySpawnData;

		public LoadEnemiesEvent(List<SaveStateController.EnemyLoadData> enemySpawnData) => EnemySpawnData = enemySpawnData;
	}
}