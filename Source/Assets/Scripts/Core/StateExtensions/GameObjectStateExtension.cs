using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	public static class GameObjectStateExtension
	{
		public static void Save(in this GameObjectComp comp, string propertyName, JsonTextWriter writer)
		{
			writer.WritePropertyName(propertyName);
			writer.WriteStartObject();
			writer.WritePropertyName("x");
			writer.WriteValue((float)Math.Round(comp.Value.transform.position.x, 2));
			writer.WritePropertyName("y");
			writer.WriteValue((float)Math.Round(comp.Value.transform.position.y, 2));
			writer.WriteEndObject();
		}

		public static void Load(ref this GameObjectComp comp, string propertyName, JObject jObject)
		{
			var positionObject = jObject.Value<JObject>(propertyName);
			comp.Value.transform.position = new Vector3(positionObject.Value<float>("x"), positionObject.Value<float>("y"));
		}

		public static Vector2 LoadPosition(this JToken token, string propertyName)
		{
			var positionObject = token.Value<JObject>(propertyName);
			return new Vector2(positionObject.Value<float>("x"), positionObject.Value<float>("y"));
		}
	}
}