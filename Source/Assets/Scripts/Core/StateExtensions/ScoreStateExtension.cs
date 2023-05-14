using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RoguelikeEcs.Core
{
	public static class ScoreStateExtension
	{
		public static void Save(in this ScoreComp comp, JsonTextWriter writer)
		{
			writer.WritePropertyName("score");
			writer.WriteStartObject();
			writer.WritePropertyName("kills");
			writer.WriteValue(comp.Kills);
			writer.WritePropertyName("bossesKilled");
			writer.WriteValue(comp.BossesKilled);
			writer.WritePropertyName("damageDealt");
			writer.WriteValue(comp.DamageDealt);
			writer.WritePropertyName("damageTaken");
			writer.WriteValue(comp.DamageTaken);
			writer.WritePropertyName("highestKillStreak");
			writer.WriteValue(comp.HighestKillStreak);
			writer.WritePropertyName("totalScore");
			writer.WriteValue(comp.TotalScore);
			writer.WritePropertyName("totalKills");
			writer.WriteValue(comp.TotalKills);
			writer.WritePropertyName("totalBossesKilled");
			writer.WriteValue(comp.TotalBossesKilled);
			writer.WritePropertyName("totalDamageDealt");
			writer.WriteValue(comp.TotalDamageDealt);
			writer.WritePropertyName("totalDamageTaken");
			writer.WriteValue(comp.TotalDamageTaken);
			writer.WritePropertyName("highestKillStreakEver");
			writer.WriteValue(comp.HighestKillStreakEver);
			writer.WriteEndObject();
		}

		public static void Load(ref this ScoreComp comp, JObject jObject)
		{
			comp.Kills = jObject.Value<int>("kills");
			comp.BossesKilled = jObject.Value<int>("bossesKilled");
			comp.DamageDealt = jObject.Value<float>("damageDealt");
			comp.DamageTaken = jObject.Value<float>("damageTaken");
			comp.HighestKillStreak = jObject.Value<int>("highestKillStreak");
			comp.TotalScore = jObject.Value<float>("totalScore");
			comp.TotalKills = jObject.Value<int>("totalKills");
			comp.TotalBossesKilled = jObject.Value<int>("totalBossesKilled");
			comp.TotalDamageDealt = jObject.Value<float>("totalDamageDealt");
			comp.TotalDamageTaken = jObject.Value<float>("totalDamageTaken");
			comp.HighestKillStreakEver = jObject.Value<int>("highestKillStreakEver");
		}
	}
}