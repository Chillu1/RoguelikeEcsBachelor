using System.Runtime.CompilerServices;
using DefaultEcs;
using UnityEngine;

[assembly: InternalsVisibleTo("RoguelikeEcs.Tests")]
[assembly: InternalsVisibleTo("RoguelikeEcs.Editor")]

namespace RoguelikeEcs.Core
{
	public sealed class GameController
	{
		public readonly World World;

		public readonly UpgradeController UpgradeSystem;
		private readonly CoreSystem<float> _uiSystem;
		public readonly CoreSystem<float> GameUpdateSystem;
		public readonly SaveStateController SaveStateController;
		private readonly PauseController _pauseController;
		private readonly UIController _uiController;

		public GameController(bool automaticAim, AreaSizeType areaSizeType, DifficultyType difficulty, CharacterType character,
			DataController dataController)
		{
			Physics2D.simulationMode = SimulationMode2D.Script;

			dataController.GameData.Unsubscribe();
			dataController.GameStateController.Unsubscribe();

			World = new World(100_000);

			dataController.GameData.Subscribe(World);
			dataController.GameStateController.Subscribe(World);

			if (!string.IsNullOrEmpty(dataController.CurrentSaveName) &&
			    SaveStateController.LoadGameData(dataController.CurrentSaveName, out var data))
			{
				areaSizeType = data.AreaSizeType;
				difficulty = data.DifficultyType;
				character = data.CharacterType;
			}

			var sceneController = new SceneController(World);

			_pauseController = new PauseController(World);
			var scoreController = new ScoreController(World);
			//Currently needs to be after scoreController because of the event subscription
			SaveStateController = new SaveStateController(World);
			UnlocksController.SubscribeUnlocks(World, dataController.UpgradesData, dataController.AchievementsData,
				dataController.CharacterData, dataController.GameData);
			UpgradeSystem = new UpgradeController(World, dataController.UpgradesData);

			_uiSystem = Core.GetUISystem(World, _pauseController);
			GameUpdateSystem = Core.GetGameLoopSystem(World, areaSizeType, difficulty, dataController);

			World.Get<UpgradesComp>().AutomaticAim = automaticAim;

			_uiController = new UIController(World, dataController.WaveData);

			var player = PlayerSpawner.Spawn(World, dataController.CharacterData.GetCharacterInfo(character), 0, 0);

			World.Publish(new PlayerResumeEvent());
			World.Publish(new ResumeActionEvent());

			if (string.IsNullOrEmpty(dataController.CurrentSaveName))
				SaveStateController.SetNewGameSave();
			else
				SaveStateController.Load(dataController.CurrentSaveName);

			player.Get<MultiplierComp>().MultipliersList.Add(new MultiplierData
				{ StatType = StatType.Damage, Multiplier = difficulty.GetPlayerDamageMultiplier() });
		}

		public void Update(float delta)
		{
			_uiSystem.Update(delta);
			_uiController.Update(delta);
			if (_pauseController.IsPaused)
				return;

			GameUpdateSystem.Update(delta);
		}

		public void FixedUpdate(float delta)
		{
			if (_pauseController.IsPaused)
				return;

			Physics2D.Simulate(delta);
		}
	}
}