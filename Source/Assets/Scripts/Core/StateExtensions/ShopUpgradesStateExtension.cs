using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RoguelikeEcs.Core
{
	public static class ShopUpgradesStateExtension
	{
		public static void Save(in this ShopUpgradesComp comp, JsonTextWriter writer)
		{
			writer.WritePropertyName("shopUpgrades");
			writer.WriteStartArray();
			for (int i = 0; i < comp.Upgrades.Length; i++)
			{
				writer.WriteStartObject();
				writer.WritePropertyName("upgradeType");
				writer.WriteValue(comp.Upgrades[i].ToString());
				writer.WritePropertyName("isBought");
				writer.WriteValue(comp.IsBought[i]);
				writer.WriteEndObject();
			}

			writer.WriteEndArray();
		}

		public static void Load(this ref ShopUpgradesComp comp, JObject jObject)
		{
			var shopUpgrades = jObject.Value<JArray>("shopUpgrades");
			for (int i = 0; i < shopUpgrades.Count; i++)
			{
				var upgrade = shopUpgrades[i].Value<JObject>();
				var upgradeType = Enum.Parse<UpgradeType>(upgrade.Value<string>("upgradeType"));
				bool isBought = upgrade.Value<bool>("isBought");
				comp.Upgrades[i] = upgradeType;
				comp.IsBought[i] = isBought;
			}
		}
	}
}