using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RoguelikeEcs.Core
{
	public static class SplitStateExtension
	{
		public static void Save(in this SplitComp comp, JsonTextWriter writer)
		{
			writer.WritePropertyName("split");
			writer.WriteValue(comp.SplitsLeft);
		}

		public static int LoadSplit(this JToken token)
		{
			return token.Value<int>("split");
		}
	}
}