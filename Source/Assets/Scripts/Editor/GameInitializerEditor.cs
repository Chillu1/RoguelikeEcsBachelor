using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DefaultEcs.System;
using RoguelikeEcs.Core;
using UnityEditor;
using UnityEngine;

namespace RoguelikeEcs.Editor
{
	[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
	[CustomEditor(typeof(GameInitializer))]
	public class GameInitializerEditor : UnityEditor.Editor
	{
		private GameInitializer _gameInitializer;
		private UpgradeType[] _upgradeTypes;
		private ISystem<float>[] _systems;
		private bool[] _enabledSystems;
		private EnemyType[] _enemyTypes;
		private EnemyType _selectedEnemyType;
		private bool _automaticTargeting;
		private bool _disableWaveSystem;

		private int _selectedUpgradeType;

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			GUILayout.Space(10);

			if (_gameInitializer == null)
			{
				_gameInitializer = (GameInitializer)target;
				_upgradeTypes = Enum.GetValues(typeof(UpgradeType)).Cast<UpgradeType>().Skip(1).ToArray();
				_systems = _gameInitializer.GameController?.GameUpdateSystem.GetSystems();
				_enabledSystems = _systems?.Select(s => s.IsEnabled).ToArray();
				_enemyTypes = Enum.GetValues(typeof(EnemyType)).Cast<EnemyType>().Skip(1).ToArray();
				_selectedEnemyType = _enemyTypes[0];
			}

			GUILayout.Space(10);
			if (_gameInitializer.GameController != null)
			{
				ref var upgrades = ref _gameInitializer.GameController.World.Get<UpgradesComp>();
				_automaticTargeting = EditorGUILayout.Toggle("Automatic targeting", upgrades.AutomaticAim);

				upgrades.AutomaticAim = _automaticTargeting;
			}

			if (GUILayout.Button("Kill Player"))
				_gameInitializer.GameController.World.GetEntities().With<IsPlayerComp>().AsSet().GetFirst().Get<HealthComp>().Current =
					-10;
			GUILayout.Space(10);

			_selectedEnemyType = (EnemyType)EditorGUILayout.EnumPopup("Enemy Type", _selectedEnemyType);

			if (GUILayout.Button("Spawn Enemy"))
				_gameInitializer.GameController.World.Publish(new SpawnEnemyEvent(_selectedEnemyType));

			if (GUILayout.Button("Kill All Enemies"))
				_gameInitializer.GameController.World.Publish(new KillAllEnemiesEvent());

			_disableWaveSystem = EditorGUILayout.Toggle("Wave System Disabled", _disableWaveSystem);
			//if (newWaveSystemEnabled != _waveSystemEnabled)
			//	_waveSystemEnabled = newWaveSystemEnabled;

			if (GUILayout.Button("Next Wave"))
				_gameInitializer.GameController.GameUpdateSystem.GetSystem<WaveSystem>().IncrementWave();

			if (_gameInitializer.GameController != null)
			{
				int newWave = EditorGUILayout.IntField("Wave", _gameInitializer.GameController.World.Get<WaveInfoComp>().WaveNumber);
				if (newWave != _gameInitializer.GameController.World.Get<WaveInfoComp>().WaveNumber)
					_gameInitializer.GameController.GameUpdateSystem.GetSystem<WaveSystem>().SetWave(newWave - 1);
			}

			if (GUILayout.Button("Finish Wave"))
				_gameInitializer.GameController.GameUpdateSystem.GetSystem<WaveSystem>().EndWave();

			GUILayout.Space(10);
			GUILayout.Label("Shop");
			if (GUILayout.Button("Get 1000 Gold"))
			{
				_gameInitializer.GameController.World.Get<GoldComp>().Value += 1000;
				_gameInitializer.GameController.World.Publish(new ScoreUpdatedEvent());
			}

			GUILayout.Space(10);
			GUILayout.Label("State");
			if (GUILayout.Button("Save"))
				_gameInitializer.GameController.SaveStateController.SaveCurrent();
			string saveName = EditorGUILayout.TextField("Save Name", _gameInitializer.GameController?.SaveStateController.CurrentSaveName);
			if (saveName != _gameInitializer.GameController?.SaveStateController.CurrentSaveName)
				_gameInitializer.GameController.SaveStateController.Load(saveName);

			GUILayout.Space(10);
			GUILayout.Label("Scenarios");
			if (GUILayout.Button("Wave 25"))
			{
				_gameInitializer.GameController.GameUpdateSystem.GetSystem<WaveSystem>().SetWave(25);
				UpgradeType[] upgradeTypes =
				{
					UpgradeType.Damage, UpgradeType.Damage, UpgradeType.Damage, UpgradeType.Damage,
					UpgradeType.Health, UpgradeType.Health, UpgradeType.Health, UpgradeType.Health,
					UpgradeType.MoveSpeed, UpgradeType.MoveSpeed, UpgradeType.MoveSpeed,
					UpgradeType.AttackSpeed, UpgradeType.AttackSpeed, UpgradeType.AttackSpeed,
					UpgradeType.BulletDamage, UpgradeType.BulletDamage, UpgradeType.BulletDamage,
					UpgradeType.AoERadius, UpgradeType.AoERadius, UpgradeType.AoERadius,
					UpgradeType.CloseDamage, UpgradeType.FarDamage, UpgradeType.CullingProjectiles,
					UpgradeType.Regen5FlatLifesteal0Multiplier, UpgradeType.CullingProjectiles
				};
				foreach (var upgradeType in upgradeTypes)
					_gameInitializer.GameController.UpgradeSystem.ApplyAndSaveUpgrade(upgradeType);
			}

			if (GUILayout.Button("Wave 50"))
			{
				_gameInitializer.GameController.GameUpdateSystem.GetSystem<WaveSystem>().SetWave(50);
				UpgradeType[] upgradeTypes =
				{
					UpgradeType.Damage, UpgradeType.Damage, UpgradeType.Damage, UpgradeType.Damage,
					UpgradeType.Damage, UpgradeType.Damage, UpgradeType.Damage, UpgradeType.Damage,
					UpgradeType.Health, UpgradeType.Health, UpgradeType.Health, UpgradeType.Health,
					UpgradeType.Health, UpgradeType.Health, UpgradeType.Health, UpgradeType.Health,
					UpgradeType.MoveSpeed, UpgradeType.MoveSpeed, UpgradeType.MoveSpeed,
					UpgradeType.MoveSpeed, UpgradeType.MoveSpeed, UpgradeType.MoveSpeed,
					UpgradeType.AttackSpeed, UpgradeType.AttackSpeed, UpgradeType.AttackSpeed,
					UpgradeType.AttackSpeed, UpgradeType.AttackSpeed, UpgradeType.AttackSpeed,
					UpgradeType.BulletDamage, UpgradeType.BulletDamage, UpgradeType.BulletDamage,
					UpgradeType.BulletDamage, UpgradeType.BulletDamage, UpgradeType.BulletDamage,
					UpgradeType.AoERadius, UpgradeType.AoERadius, UpgradeType.AoERadius,
					UpgradeType.AoERadius, UpgradeType.AoERadius, UpgradeType.AoERadius,
					UpgradeType.CloseDamage, UpgradeType.FarDamage, UpgradeType.CullingProjectiles,
					UpgradeType.Regen5FlatLifesteal0Multiplier, UpgradeType.CullingProjectiles,
					UpgradeType.HomingProjectiles, UpgradeType.HomingProjectiles, UpgradeType.HomingProjectiles,
					UpgradeType.Pierce10FlatDamage0_5Multiplier
				};
				foreach (var upgradeType in upgradeTypes)
					_gameInitializer.GameController.UpgradeSystem.ApplyAndSaveUpgrade(upgradeType);
			}

			if (GUILayout.Button("Wave 150"))
			{
				_gameInitializer.GameController.GameUpdateSystem.GetSystem<WaveSystem>().SetWave(150);
				UpgradeType[] upgradeTypes =
				{
					UpgradeType.Damage, UpgradeType.Damage, UpgradeType.Damage, UpgradeType.Damage, UpgradeType.Damage, UpgradeType.Damage,
					UpgradeType.Damage, UpgradeType.Damage, UpgradeType.Damage, UpgradeType.Damage, UpgradeType.Damage, UpgradeType.Damage,
					UpgradeType.Damage, UpgradeType.Damage, UpgradeType.Damage, UpgradeType.Damage, UpgradeType.Damage, UpgradeType.Damage,
					UpgradeType.Health, UpgradeType.Health, UpgradeType.Health, UpgradeType.Health, UpgradeType.Health, UpgradeType.Health,
					UpgradeType.Health, UpgradeType.Health, UpgradeType.Health, UpgradeType.Health, UpgradeType.Health, UpgradeType.Health,
					UpgradeType.MoveSpeed, UpgradeType.MoveSpeed, UpgradeType.MoveSpeed,
					UpgradeType.MoveSpeed, UpgradeType.MoveSpeed, UpgradeType.MoveSpeed,
					UpgradeType.MoveSpeed, UpgradeType.MoveSpeed, UpgradeType.MoveSpeed,
					UpgradeType.AttackSpeed, UpgradeType.AttackSpeed, UpgradeType.AttackSpeed,
					UpgradeType.AttackSpeed, UpgradeType.AttackSpeed, UpgradeType.AttackSpeed,
					UpgradeType.AttackSpeed, UpgradeType.AttackSpeed, UpgradeType.AttackSpeed,
					UpgradeType.BulletDamage, UpgradeType.BulletDamage, UpgradeType.BulletDamage,
					UpgradeType.BulletDamage, UpgradeType.BulletDamage, UpgradeType.BulletDamage,
					UpgradeType.BulletDamage, UpgradeType.BulletDamage, UpgradeType.BulletDamage,
					UpgradeType.AoERadius, UpgradeType.AoERadius, UpgradeType.AoERadius,
					UpgradeType.AoERadius, UpgradeType.AoERadius, UpgradeType.AoERadius,
					UpgradeType.AoERadius, UpgradeType.AoERadius, UpgradeType.AoERadius,
					UpgradeType.CloseDamage, UpgradeType.FarDamage, UpgradeType.CullingProjectiles,
					UpgradeType.Regen5FlatLifesteal0Multiplier, UpgradeType.CullingProjectiles,
					UpgradeType.HomingProjectiles, UpgradeType.HomingProjectiles, UpgradeType.HomingProjectiles,
					UpgradeType.Pierce10FlatDamage0_5Multiplier
				};
				foreach (var upgradeType in upgradeTypes)
					_gameInitializer.GameController.UpgradeSystem.ApplyAndSaveUpgrade(upgradeType);
			}

			if (GUILayout.Button("Wave Hell"))
			{
				_gameInitializer.GameController.GameUpdateSystem.GetSystem<WaveSystem>().SetWave(200);
				UpgradeType[] upgradeTypes =
				{
					UpgradeType.Damage, UpgradeType.Damage, UpgradeType.Damage, UpgradeType.Damage, UpgradeType.Damage, UpgradeType.Damage,
					UpgradeType.Damage, UpgradeType.Damage, UpgradeType.Damage, UpgradeType.Damage, UpgradeType.Damage, UpgradeType.Damage,
					UpgradeType.Damage, UpgradeType.Damage, UpgradeType.Damage, UpgradeType.Damage, UpgradeType.Damage, UpgradeType.Damage,
					UpgradeType.Health, UpgradeType.Health, UpgradeType.Health, UpgradeType.Health, UpgradeType.Health, UpgradeType.Health,
					UpgradeType.Health, UpgradeType.Health, UpgradeType.Health, UpgradeType.Health, UpgradeType.Health, UpgradeType.Health,
					UpgradeType.MoveSpeed, UpgradeType.MoveSpeed, UpgradeType.MoveSpeed,
					UpgradeType.MoveSpeed, UpgradeType.MoveSpeed, UpgradeType.MoveSpeed,
					UpgradeType.MoveSpeed, UpgradeType.MoveSpeed, UpgradeType.MoveSpeed,
					UpgradeType.AttackSpeed, UpgradeType.AttackSpeed, UpgradeType.AttackSpeed, UpgradeType.AttackSpeed,
					UpgradeType.AttackSpeed, UpgradeType.AttackSpeed, UpgradeType.AttackSpeed, UpgradeType.AttackSpeed,
					UpgradeType.AttackSpeed, UpgradeType.AttackSpeed, UpgradeType.AttackSpeed, UpgradeType.AttackSpeed,
					UpgradeType.AttackSpeed, UpgradeType.AttackSpeed, UpgradeType.AttackSpeed, UpgradeType.AttackSpeed,
					UpgradeType.BulletDamage, UpgradeType.BulletDamage, UpgradeType.BulletDamage,
					UpgradeType.BulletDamage, UpgradeType.BulletDamage, UpgradeType.BulletDamage,
					UpgradeType.BulletDamage, UpgradeType.BulletDamage, UpgradeType.BulletDamage,
					UpgradeType.AoERadius, UpgradeType.AoERadius, UpgradeType.AoERadius,
					UpgradeType.AoERadius, UpgradeType.AoERadius, UpgradeType.AoERadius,
					UpgradeType.AoERadius, UpgradeType.AoERadius, UpgradeType.AoERadius,
					UpgradeType.CloseDamage, UpgradeType.FarDamage, UpgradeType.CullingProjectiles,
					UpgradeType.Regen5FlatLifesteal0Multiplier, UpgradeType.CullingProjectiles,
					UpgradeType.HomingProjectiles, UpgradeType.HomingProjectiles, UpgradeType.HomingProjectiles,
					UpgradeType.Pierce10FlatDamage0_5Multiplier
				};
				foreach (var upgradeType in upgradeTypes)
					_gameInitializer.GameController.UpgradeSystem.ApplyAndSaveUpgrade(upgradeType);
			}

			GUILayout.Space(10);
			GUILayout.Label("Upgrades");

			if (GUILayout.Button("Get All Upgrades"))
			{
				foreach (var upgradeType in _upgradeTypes)
					_gameInitializer.GameController.World.Publish(new UpgradeChosenEvent(upgradeType));
			}

			_selectedUpgradeType = GUILayout.SelectionGrid(_selectedUpgradeType, _upgradeTypes.Select(x => x.ToString()).ToArray(), 2);
			if (GUILayout.Button("Get Selected Upgrade"))
				_gameInitializer.GameController.World.Publish(new UpgradeChosenEvent(_upgradeTypes[_selectedUpgradeType]));

			GUILayout.Space(10);
			GUILayout.Label("Systems");

			if (_systems != null)
			{
				for (int i = 0; i < _systems.Length; i++)
				{
					ISystem<float> system = _systems[i];
					bool enabled = _enabledSystems![i];
					var guiStyle = new GUIStyle(GUI.skin.button);
					bool newEnabled = EditorGUILayout.Toggle(system.GetType().Name, enabled, guiStyle, GUILayout.Width(250));
					if (newEnabled != enabled)
					{
						system.IsEnabled = newEnabled;
						_enabledSystems[i] = newEnabled;
					}

					if (system is WaveSystem waveSystem)
						waveSystem.IsEnabled = !_disableWaveSystem;
				}
			}
		}
	}
}