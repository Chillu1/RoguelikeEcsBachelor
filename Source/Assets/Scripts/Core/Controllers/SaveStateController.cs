using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using DefaultEcs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoguelikeEcs.Core.Utilities;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace RoguelikeEcs.Core
{
	public sealed class SaveStateController
	{
		private readonly World _world;

		private static readonly Version saveVersion = new Version(0, 1, 0);
		private static readonly string defaultSavePath
#if UNITY_WEBGL && !UNITY_EDITOR
			= "idbfs/" + Application.companyName + "/" + Application.productName + "/saves/";
#else
			= Application.persistentDataPath + "/saves/";
#endif
		public const string DefaultSaveFileName = "save.json";

		public string CurrentSaveName { get; private set; }
		private double _filePlayTime;

		public SaveStateController(World world)
		{
			_world = world;

			world.Subscribe((in WaveCompletedEvent _) => SaveCurrent());
			world.Subscribe<GameOverEvent>(RenameSave);
		}

		public static bool SaveExists(string fileName)
		{
			bool exists = File.Exists(defaultSavePath + fileName);
			if (!exists)
				Debug.LogError($"Save {fileName} does not exist. At path {defaultSavePath}");
			return exists;
		}

		public void SetNewGameSave()
		{
			int i = 1;
			string saveName = DefaultSaveFileName;
			while (File.Exists(defaultSavePath + saveName))
			{
				saveName = DefaultSaveFileName.Substring(0, DefaultSaveFileName.IndexOf(".json", StringComparison.Ordinal)) + i + ".json";
				i++;
			}

			CurrentSaveName = saveName;
		}

		public bool SaveCurrent() => Save(CurrentSaveName);

		public bool Save() => Save(DefaultSaveFileName);

		public bool Save(string fileName)
		{
			string directory = defaultSavePath;

			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);

			if (!fileName.EndsWith(".json"))
				fileName += ".json";

			string fullSavePath = directory + fileName;
			double playTime = Time.timeSinceLevelLoadAsDouble;

			if (File.Exists(fullSavePath))
			{
				using var streamReader = new StreamReader(fullSavePath);
				using var reader = new JsonTextReader(streamReader);

				var saveData = JObject.Load(reader); //TODO Parsing the whole file is a bit overkill
				reader.Close();

				if (_filePlayTime == 0d)
					_filePlayTime = saveData.Value<double>("playTime");
			}

			using var streamWriter = new StreamWriter(fullSavePath, false);
			if (SaveInternal(streamWriter, fileName.Remove(fileName.IndexOf(".json", StringComparison.Ordinal)), _filePlayTime + playTime))
			{
				Debug.Log("Save state save successful");
				return true;
			}

			return false;
		}

		public static bool LoadGameData(string fileName, out SaveGameData data)
		{
			data = default;
			string directory = defaultSavePath;

			if (!Directory.Exists(directory))
			{
				Debug.LogError($"Load directory {directory} does not exist");
				return false;
			}

			if (!fileName.EndsWith(".json"))
				fileName += ".json";

			string fullSavePath = directory + fileName;

			if (!File.Exists(fullSavePath))
			{
				Debug.LogError($"File {fileName} does not exist");
				return false;
			}

			try
			{
				using var reader = new JsonTextReader(new StreamReader(fullSavePath));

				var saveData = JObject.Load(reader);
				reader.Close();

				var areaSize = Enum.Parse<AreaSizeType>(saveData.Value<string>("areaSize"));
				var difficulty = Enum.Parse<DifficultyType>(saveData.Value<string>("difficulty"));
				var characterType = saveData.LoadCharacterType();
				data = new SaveGameData(areaSize, difficulty, characterType);

				Debug.Log("Load save game data successful");
				return true;
			}
			catch (Exception e)
			{
				Debug.LogError("Invalid/corrupt save");
				Debug.LogException(e);
				return false;
			}
		}

		public static string GetLatestSaveName()
		{
			//Gets latest save file name, by number. save1, save2, etc
			string directory = defaultSavePath;

			if (!Directory.Exists(directory))
			{
				Debug.LogError($"Load directory {directory} does not exist");
				return null;
			}

			string[] files = Directory.GetFiles(directory, "save*.json");
			if (files.Length == 0)
			{
				Debug.LogError("No save files found");
				return DefaultSaveFileName;
			}

			string latestSave = DefaultSaveFileName;
			int latestNumber = 0;
			foreach (string file in files)
			{
				string fileName = Path.GetFileNameWithoutExtension(file);
				string numbers = Regex.Match(fileName, @"\d+$", RegexOptions.RightToLeft).Value;
				if (int.TryParse(numbers, out int number))
				{
					if (number > latestNumber)
					{
						latestNumber = number;
						latestSave = fileName + ".json";
					}
				}
			}

			Debug.Log($"Latest save is {latestSave}");
			return latestSave;
		}

		public bool Load(string fileName)
		{
			string directory = defaultSavePath;

			if (!Directory.Exists(directory))
			{
				Debug.LogError($"Load directory {directory} does not exist");
				return false;
			}

			if (!fileName.EndsWith(".json"))
				fileName += ".json";

			string fullSavePath = directory + fileName;

			if (!File.Exists(fullSavePath))
			{
				Debug.LogError($"File {fileName} does not exist");
				return false;
			}

			using var streamReader = new StreamReader(fullSavePath);
			if (LoadInternal(streamReader, fileName))
			{
				Debug.Log("Load successful");
				return true;
			}

			return false;
		}

		private bool SaveInternal(TextWriter textWriter, string saveName, double playTime)
		{
			try
			{
				using var writer = new JsonTextWriter(textWriter);
				writer.Formatting = Formatting.Indented;

				writer.WriteStartObject();
				writer.WriteValue("saveName", saveName);
				writer.WriteValue("gameVersion", DataController.Version.ToString());
				writer.WriteValue("saveVersion", saveVersion.ToString());
				writer.WriteValue("playTime", (float)Math.Round(playTime, 1));
				writer.WriteValue("saveDate", DateTime.UtcNow);

				writer.WriteValue("automaticAim", _world.Get<UpgradesComp>().AutomaticAim);
				_world.Get<AreaSizeComp>().Save(writer);
				_world.Get<DifficultyComp>().Save(writer);

				_world.Get<WaveInfoComp>().Save(writer);
				_world.Get<ShopUpgradesComp>().Save(writer);

				_world.Get<ScoreComp>().Save(writer);
				_world.Get<GoldComp>().Save(writer);

				ref readonly var player = ref _world.GetEntities().With<IsPlayerComp>().AsSet().GetFirst();
				player.Get<CharacterTypeComp>().Save(writer);
				player.Get<GameObjectComp>().Save("playerPosition", writer);
				player.Get<HealthComp>().Save("playerHealthPercent", writer);

				_world.Get<UpgradesComp>().Save(writer);

				var enemies = _world.GetEntities().With<IsEnemy>().AsSet();
				writer.WritePropertyName("enemies");
				writer.WriteStartArray();
				foreach (ref readonly var enemy in enemies.GetEntities())
				{
					//Saves type, position, health. Disregards any other state (for now)
					writer.WriteStartObject();
					enemy.Get<EnemyTypeComp>().Save(writer);
					enemy.Get<GameObjectComp>().Save("position", writer);
					enemy.Get<HealthComp>().Save("health", writer);
					if (enemy.TryGet<SplitComp>(out var split))
						split.Save(writer);
					writer.WriteEndObject();
				}

				writer.WriteEndArray();

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

		private bool LoadInternal(TextReader textReader, string fileName)
		{
			try
			{
				using var reader = new JsonTextReader(textReader);

				var saveData = JObject.Load(reader);
				reader.Close();
				var version = Version.Parse(saveData.Value<string>("gameVersion"));
				if (version != DataController.Version)
					Debug.LogWarning("Saved game version is different from current game version.");
				var saveVersion = Version.Parse(saveData.Value<string>("saveVersion"));
				if (saveVersion != SaveStateController.saveVersion)
					Debug.LogWarning("Saved save version is different from current save version. Might need to update?");

				ref var upgrades = ref _world.Get<UpgradesComp>();
				upgrades.AutomaticAim = saveData.Value<bool>("automaticAim");

				ref var waveInfoComp = ref _world.Get<WaveInfoComp>();
				waveInfoComp.Load(saveData);
				_world.Get<ShopUpgradesComp>().Load(saveData);

				_world.Get<ScoreComp>().Load(saveData);
				_world.Get<GoldComp>().Load(saveData);

				var player = _world.GetEntities().With<IsPlayerComp>().AsSet().GetFirst();
				player.Get<GameObjectComp>().Load("playerPosition", saveData);
				player.Get<HealthComp>().Load("playerHealthPercent", saveData);

				upgrades.Load(saveData);

				var enemySpawnDataList = new List<EnemyLoadData>();
				var enemiesData = saveData.Value<JArray>("enemies");
				foreach (var enemyData in enemiesData)
				{
					enemySpawnDataList.Add(new EnemyLoadData
					{
						EnemyType = enemyData.LoadEnemyType(),
						Position = enemyData.LoadPosition("position"),
						HealthPercent = enemyData.LoadHealth("health"),
						SplitCount = enemyData.LoadSplit() //Will trigger for every non split enemy, but that's fine
					});
				}

				CurrentSaveName = saveData.Value<string>("saveName");
				if (string.IsNullOrEmpty(CurrentSaveName))
					CurrentSaveName = fileName;

				_world.Publish(new LoadUpgradesEvent(upgrades.UpgradesData));
				//Debug.Log("Upgrade Data: " + string.Join(", ", upgrades.UpgradesData.Select(x => x.ToString()).ToArray()));

				_world.Publish(new LoadEnemiesEvent(enemySpawnDataList));

				if (waveInfoComp.WaveFinished)
					_world.Publish(new FinishWaveEvent());

				var position = player.Get<GameObjectComp>().Value.transform.position;
				var cameraTransform = Camera.main.transform;
				cameraTransform.position = Vector3.Lerp(cameraTransform.position, new Vector3(position.x, position.y, -10), 0.6f);
			}
			catch (Exception e)
			{
				Debug.LogError("Invalid/corrupt save");
				Debug.LogException(e);
				return false;
			}

			return true;
		}

		//Temporary solution to deleting save files. In case we want to keep them for some reason/not have permanent death
		private void RenameSave(in GameOverEvent gameOverEvent)
		{
			if (!Save(CurrentSaveName))
			{
				Debug.LogError("Failed to save before deleting save file");
				return;
			}

			//rename current save from save*.json to deadsave*.json
			string saveName = CurrentSaveName;
			string savePath = Path.Combine(defaultSavePath, saveName);
			string deadSavePath = Path.Combine(defaultSavePath, "dead" + saveName);
			if (File.Exists(savePath))
			{
				if (File.Exists(deadSavePath))
					File.Delete(deadSavePath);
				File.Move(savePath, deadSavePath);
			}
		}

		public struct SaveGameData
		{
			public AreaSizeType AreaSizeType;
			public DifficultyType DifficultyType;
			public CharacterType CharacterType;

			public SaveGameData(AreaSizeType areaSize, DifficultyType difficulty, CharacterType characterType)
			{
				AreaSizeType = areaSize;
				DifficultyType = difficulty;
				CharacterType = characterType;
			}
		}

		public struct EnemyLoadData
		{
			public EnemyType EnemyType;
			public Vector2 Position;
			public float HealthPercent;
			public int SplitCount;
		}
	}
}