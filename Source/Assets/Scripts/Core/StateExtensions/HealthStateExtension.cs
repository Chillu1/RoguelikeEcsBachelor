using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RoguelikeEcs.Core
{
	public static class HealthStateExtension
	{
		public static void Save(in this HealthComp comp, string propertyName, JsonTextWriter writer)
		{
			if (comp.IsMax || comp.Percent == 0)
				return;

			writer.WritePropertyName(propertyName);
			writer.WriteValue((float)Math.Round(comp.Percent, 2));
		}

		public static void Load(ref this HealthComp comp, string propertyName, JObject jObject)
		{
			if (!jObject.ContainsKey(propertyName))
			{
				comp.Current = comp.Max;
				return;
			}

			float healthPercent = jObject.Value<float>(propertyName);
			comp.Current = healthPercent * comp.Max;
		}

		public static float LoadHealth(this JToken token, string propertyName)
		{
			float healthPercent = token.Value<float>(propertyName);
			if (healthPercent == 0)
				return 1;

			return healthPercent;
		}
	}
}