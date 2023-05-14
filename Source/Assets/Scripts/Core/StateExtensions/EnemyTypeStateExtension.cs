using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	public static class EnemyTypeStateExtension
	{
		public static void Save(in this EnemyTypeComp comp, JsonTextWriter writer)
		{
			writer.WritePropertyName("type");
			writer.WriteValue(comp.Value.ToString());
		}

		public static EnemyType LoadEnemyType(this JToken token)
		{
			string enemyType = token.Value<string>("type");
			if (Enum.TryParse(enemyType, out EnemyType enemyTypeEnum))
				return enemyTypeEnum;

			Debug.LogError($"Could not parse enemy type {enemyType}. Defaulting to Basic.");
			return EnemyType.Basic;
		}
	}
}