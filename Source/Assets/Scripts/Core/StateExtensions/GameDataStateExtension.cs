using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	public static class GameDataStateExtension
	{
		public static void Save(this GameData gameData, JsonTextWriter writer)
		{
			writer.WritePropertyName("totalPlayTime");
			writer.WriteValue(gameData.TotalPlayTime);
			writer.WritePropertyName("totalKills");
			writer.WriteValue(gameData.TotalKills);
			writer.WritePropertyName("totalBossKills");
			writer.WriteValue(gameData.TotalBossKills);
			writer.WritePropertyName("totalDamage");
			writer.WriteValue(gameData.TotalDamage);
			writer.WritePropertyName("totalAoEDamage");
			writer.WriteValue(gameData.TotalAoEDamage);
			writer.WritePropertyName("highestKillStreak");
			writer.WriteValue(gameData.HighestKillStreak);
			writer.WritePropertyName("projectilesFired");
			writer.WriteValue(gameData.ProjectilesFired);
			writer.WritePropertyName("missedProjectiles");
			writer.WriteValue(gameData.MissedProjectiles);
			writer.WritePropertyName("distanceTraveled");
			writer.WriteValue(gameData.DistanceTraveled);

			writer.WritePropertyName("unlockedUpgrades");
			writer.WriteStartArray();
			foreach (var type in gameData.UnlockedUpgrades)
				writer.WriteValue(type.ToString());
			writer.WriteEndArray();

			writer.WritePropertyName("completedAchievements");
			writer.WriteStartArray();
			foreach (string achievementName in gameData.CompletedAchievements)
				writer.WriteValue(achievementName);
			writer.WriteEndArray();

			writer.WritePropertyName("unlockedCharacters");
			writer.WriteStartArray();
			foreach (var type in gameData.UnlockedCharacters)
				writer.WriteValue(type.ToString());
			writer.WriteEndArray();
		}

		public static void Load(this GameData gameData, JObject jObject)
		{
			gameData.AddPlayTime(jObject.Value<float>("totalPlayTime"));
			gameData.AddKills(jObject.Value<int>("totalKills"));
			gameData.AddBossKills(jObject.Value<int>("totalBossKills"));
			gameData.AddDamage(jObject.Value<float>("totalDamage"));
			gameData.AddAoEDamage(jObject.Value<float>("totalAoEDamage"));
			gameData.AddKillStreak(jObject.Value<int>("highestKillStreak"));
			gameData.AddProjectilesFired(jObject.Value<int>("projectilesFired"));
			if (jObject.ContainsKey("missedProjectiles")) //TODO Save system versioning
				gameData.AddMissedProjectiles(jObject.Value<int>("missedProjectiles"));
			if (jObject.ContainsKey("distanceTraveled")) //TODO Save system versioning
				gameData.AddDistanceTraveled(jObject.Value<float>("distanceTraveled"));

			gameData.UnlockedUpgrades.Clear();
			foreach (JToken token in jObject.Value<JArray>("unlockedUpgrades"))
				gameData.UnlockedUpgrades.Add(Enum.Parse<UpgradeType>(token.Value<string>()));

			gameData.CompletedAchievements.Clear();
			foreach (JToken token in jObject.Value<JArray>("completedAchievements"))
				gameData.CompletedAchievements.Add(token.Value<string>());

			gameData.UnlockedCharacters.Clear();
			foreach (JToken token in jObject.Value<JArray>("unlockedCharacters"))
				gameData.UnlockedCharacters.Add(Enum.Parse<CharacterType>(token.Value<string>()));
		}
	}
}