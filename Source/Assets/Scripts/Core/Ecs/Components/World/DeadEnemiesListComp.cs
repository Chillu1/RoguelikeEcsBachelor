using System.Collections.Generic;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	public struct DeadEnemiesListComp
	{
		public List<DeadEnemyInfo> Value;

		public struct DeadEnemyInfo
		{
			public EnemyType EnemyType;
			public Vector2 Position;
		}
	}
}