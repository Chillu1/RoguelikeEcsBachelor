using System.Collections.Generic;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	public static class EnemyData //TODO MoveMe/RenameMe?
	{
		private static Dictionary<EnemyType, int> _enemyScores;

		static EnemyData() => _enemyScores = new Dictionary<EnemyType, int>();

		public static void SetupScores(Dictionary<EnemyType, int> scores)
		{
			_enemyScores = scores;
		}

		public static int GetScore(EnemyType type)
		{
			if (_enemyScores.ContainsKey(type))
				return _enemyScores[type];

			Debug.LogError("EnemyData.GetScore: No score for enemy type " + type);
			return 100;
		}
	}
}