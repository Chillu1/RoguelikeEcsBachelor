using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RoguelikeEcs.Core
{
	public static class WaveInfoStateExtension
	{
		public static void Save(in this WaveInfoComp comp, JsonTextWriter writer)
		{
			writer.WritePropertyName("waveInfo");
			writer.WriteStartObject();
			writer.WritePropertyName("time");
			writer.WriteValue(comp.Time);
			writer.WritePropertyName("waveNumber");
			writer.WriteValue(comp.WaveNumber);
			writer.WritePropertyName("waveFinished");
			writer.WriteValue(comp.WaveFinished);
			writer.WriteEndObject();
		}

		public static void Load(ref this WaveInfoComp comp, JObject jObject)
		{
			var waveInfoObject = jObject.Value<JObject>("waveInfo");
			comp.Time = waveInfoObject.Value<float>("time");
			comp.WaveNumber = waveInfoObject.Value<int>("waveNumber");
			comp.WaveFinished = waveInfoObject.Value<bool>("waveFinished");
		}
	}
}