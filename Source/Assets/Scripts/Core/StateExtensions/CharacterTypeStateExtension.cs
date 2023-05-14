using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RoguelikeEcs.Core
{
	public static class CharacterTypeStateExtension
	{
		public static void Save(in this CharacterTypeComp comp, JsonTextWriter writer)
		{
			writer.WritePropertyName("characterType");
			writer.WriteValue(comp.Value.ToString());
		}

		public static CharacterType LoadCharacterType(this JToken token)
		{
			return Enum.Parse<CharacterType>(token.Value<string>("characterType"));
		}
	}
}