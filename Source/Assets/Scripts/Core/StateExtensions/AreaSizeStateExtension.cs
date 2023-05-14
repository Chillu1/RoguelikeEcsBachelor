using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RoguelikeEcs.Core
{
	public static class AreaSizeStateExtension
	{
		public static void Save(in this AreaSizeComp comp, JsonTextWriter writer)
		{
			writer.WritePropertyName("areaSize");
			writer.WriteValue(comp.Value.ToString());
		}

		public static void Load(ref this AreaSizeComp comp, JObject jObject)
		{
			comp.Value = Enum.Parse<AreaSizeType>(jObject.Value<string>("areaSize"));
		}
	}
}