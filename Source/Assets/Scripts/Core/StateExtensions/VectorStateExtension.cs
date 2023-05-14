using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	public static class VectorStateExtension
	{
		public static void Save(in this Vector2 vector, string propertyName, JsonTextWriter writer)
		{
			writer.WritePropertyName(propertyName);
			writer.WriteStartObject();
			writer.WritePropertyName("x");
			writer.WriteValue(vector.x);
			writer.WritePropertyName("y");
			writer.WriteValue(vector.y);
			writer.WriteEndObject();
		}

		public static void Load(ref this Vector2 vector, string propertyName, JObject jObject)
		{
			var vectorObject = jObject.Value<JObject>(propertyName);
			vector.x = vectorObject.Value<float>("x");
			vector.y = vectorObject.Value<float>("y");
		}
	}
}