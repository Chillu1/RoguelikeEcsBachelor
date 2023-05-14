using UnityEngine;

namespace RoguelikeEcs.Core
{
	public readonly struct EnemyHoverOverEvent
	{
		public readonly Vector3 Position;
		public readonly EnemyType EnemyType;

		public EnemyHoverOverEvent(Vector3 position, EnemyType enemyType)
		{
			Position = position;
			EnemyType = enemyType;
		}
	}
}