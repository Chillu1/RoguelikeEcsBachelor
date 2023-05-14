using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RoguelikeEcs.Core
{
	public static class DifficultyStateExtension
	{
		public static void Save(in this DifficultyComp comp, JsonTextWriter writer)
		{
			writer.WritePropertyName("difficulty");
			writer.WriteValue(comp.Value.ToString());
		}

		public static void Load(ref this DifficultyComp comp, JObject jObject)
		{
			comp.Value = Enum.Parse<DifficultyType>(jObject.Value<string>("difficulty"));
		}
	}
}