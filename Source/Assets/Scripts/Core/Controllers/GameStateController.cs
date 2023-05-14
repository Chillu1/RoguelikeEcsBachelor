using System;
using System.IO;
using DefaultEcs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoguelikeEcs.Core.Utilities;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	public sealed class GameStateController
	{
		private static readonly Version gameSaveVersion = new Version(0, 1, 0);

		private static readonly string defaultSavePath
#if UNITY_WEBGL && !UNITY_EDITOR
			= "idbfs/" + Application.companyName + "/" + Application.productName + "/";
#else
			= Application.persistentDataPath + "/";
#endif
		public const string DefaultGameSaveFileName = "game_data.json";

		private readonly GameData _gameData;

		private IDisposable _worldSubscription;

		public GameStateController(GameData gameData)
		{
			_gameData = gameData;

			if (!GameStateExists())
				SaveGameState();
			else
				LoadGameState();
		}

		public void Subscribe(World world) => _worldSubscription = world.Subscribe((in GameDataUpdatedEvent _) => Save());
		public void Unsubscribe() => _worldSubscription?.Dispose();

		public bool Save() => SaveGameState();

		private static bool GameStateExists(string savePath = "", string fileName = DefaultGameSaveFileName)
		{
			if (string.IsNullOrEmpty(savePath))
				savePath = defaultSavePath;

			if (!Directory.Exists(savePath))
			{
				Debug.LogWarning($"Load directory {savePath} does not exist");
				return false;
			}

			return File.Exists(savePath + fileName);
		}

		private bool SaveGameState(string fileName = DefaultGameSaveFileName)
		{
			string directory = defaultSavePath;

			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);

			if (!fileName.EndsWith(".json"))
				fileName += ".json";

			string fullSavePath = directory + fileName;

			using var streamWriter = new StreamWriter(fullSavePath, false);
			if (SaveInternal(streamWriter, fileName.Remove(fileName.IndexOf(".json", StringComparison.Ordinal))))
			{
				Debug.Log("Game state save successful");
				return true;
			}

			return false;
		}

		private bool LoadGameState(string fileName = DefaultGameSaveFileName)
		{
			string savePath = defaultSavePath;

			if (string.IsNullOrEmpty(savePath))
				savePath = defaultSavePath;

			if (!GameStateExists(savePath))
				return false;

			if (!fileName.EndsWith(".json"))
				fileName += ".json";

			string fullSavePath = savePath + fileName;

			if (!File.Exists(fullSavePath))
			{
				Debug.LogError($"File {fileName} does not exist");
				return false;
			}

			using var streamReader = new StreamReader(fullSavePath);
			if (LoadInternal(streamReader))
			{
				Debug.Log("Game State Load successful");
				return true;
			}

			return false;
		}

		private bool SaveInternal(TextWriter textWriter, string saveName)
		{
			try
			{
				using var writer = new JsonTextWriter(textWriter);
				writer.Formatting = Formatting.Indented;

				writer.WriteStartObject();
				writer.WriteValue("gameSaveName", saveName);
				writer.WriteValue("gameVersion", DataController.Version.ToString());
				writer.WriteValue("gameSaveVersion", gameSaveVersion.ToString());
				writer.WriteValue("saveDate", DateTime.UtcNow);

				_gameData.Save(writer);

				writer.WriteEndObject();

				writer.Close();
			}
			catch (Exception e)
			{
				Debug.LogError("Fatal. Invalid save data?");
				Debug.LogException(e);
				return false;
			}

			return true;
		}

		private bool LoadInternal(TextReader textReader)
		{
			try
			{
				using var reader = new JsonTextReader(textReader);

				var saveData = JObject.Load(reader);
				reader.Close();
				var version = Version.Parse(saveData.Value<string>("gameVersion"));
				if (version != DataController.Version)
					Debug.LogWarning("Saved game version is different from current game version.");
				var saveVersion = Version.Parse(saveData.Value<string>("gameSaveVersion"));
				if (saveVersion != gameSaveVersion)
					Debug.LogWarning("Saved save version is different from current save version. Might need to update?");

				_gameData.Load(saveData);
			}
			catch (Exception e)
			{
				Debug.LogError("Invalid/corrupt save");
				Debug.LogException(e);
				return false;
			}

			return true;
		}
	}
}