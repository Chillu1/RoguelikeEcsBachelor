using System;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	public sealed class DataController
	{
		public static Version Version { get; }

		public UpgradesData UpgradesData { get; }
		public AchievementsData AchievementsData { get; }
		public EnemyRecipes EnemyRecipes { get; }
		public WaveData WaveData { get; }
		public CharacterData CharacterData { get; }

		public GameData GameData { get; }
		public GameStateController GameStateController { get; }

		public string CurrentSaveName { get; set; }
		public bool AutomaticAim { get; set; }
		public AreaSizeType? AreaSize { get; set; }
		public DifficultyType? Difficulty { get; set; }
		public CharacterType? PlayerCharacter { get; set; }
		public bool HellWave { get; set; } //TODO TEMP

		static DataController()
		{
			Version = new Version(Application.version);
		}

		public DataController()
		{
			UpgradesData = new UpgradesData();
			AchievementsData = new AchievementsData();
			EnemyRecipes = new EnemyRecipes();
			WaveData = new WaveData();
			CharacterData = new CharacterData();

			GameData = new GameData();
			GameStateController = new GameStateController(GameData);
		}
	}
}