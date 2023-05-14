using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RoguelikeEcs.Core
{
	public static class UpgradesStateExtension
	{
		public static void Save(in this UpgradesComp comp, JsonTextWriter writer)
		{
			writer.WritePropertyName("upgrades");
			writer.WriteStartObject();
			foreach (var data in comp.UpgradesData)
			{
				writer.WritePropertyName(data.Key.ToString());
				writer.WriteValue(data.Value);
			}

			writer.WriteEndObject();
		}

		public static void Load(ref this UpgradesComp comp, JObject jObject)
		{
			var upgrades = jObject.Value<JObject>("upgrades");
			foreach (var upgrade in upgrades)
			{
				var upgradeType = (UpgradeType)Enum.Parse(typeof(UpgradeType), upgrade.Key);
				int upgradeCount = upgrade.Value.Value<int>();
				comp.UpgradesData[upgradeType] = upgradeCount;
			}
		}
	}
}