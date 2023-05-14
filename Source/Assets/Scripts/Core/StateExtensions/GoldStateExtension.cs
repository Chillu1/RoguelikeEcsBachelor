using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RoguelikeEcs.Core
{
	public static class GoldStateExtension
	{
		public static void Save(in this GoldComp comp, JsonTextWriter writer)
		{
			writer.WritePropertyName("gold");
			writer.WriteValue(comp.Value);
		}

		public static void Load(ref this GoldComp comp, JObject jObject)
		{
			comp.Value = jObject.Value<float>("gold");
		}
	}
}