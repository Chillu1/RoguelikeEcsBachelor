using UnityEngine;

namespace RoguelikeEcs.Core
{
	public readonly struct SpawnEnemyEvent
	{
		public readonly EnemyType Type;
		public readonly Vector2 Position;

		public SpawnEnemyEvent(EnemyType type) : this()
		{
			Type = type;
		}

		public SpawnEnemyEvent(EnemyType type, Vector2 position)
		{
			Type = type;
			Position = position;
		}
	}
}