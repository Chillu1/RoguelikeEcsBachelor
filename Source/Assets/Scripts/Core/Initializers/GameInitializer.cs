using DefaultEcs;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	public sealed class GameInitializer : MonoBehaviour
	{
		public GameController GameController { get; private set; }

		[Header("World")]
		public AreaSizeType AreaSize;

		public DifficultyType Difficulty;
		public CharacterType PlayerCharacter;

		public void Start()
		{
			var dataController = FindObjectOfType<DataInitializer>().DataController;
			if (dataController.AreaSize != null)
				AreaSize = (AreaSizeType)dataController.AreaSize;
			if (dataController.Difficulty != null)
				Difficulty = (DifficultyType)dataController.Difficulty;
			if (dataController.PlayerCharacter != null)
				PlayerCharacter = (CharacterType)dataController.PlayerCharacter;

			GameController = new GameController(dataController.AutomaticAim, AreaSize, Difficulty, PlayerCharacter, dataController);

			if (dataController.HellWave) //TODO TEMP
			{
				GameController.GameUpdateSystem.GetSystem<WaveSystem>().SetWave(200);
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
					GameController.UpgradeSystem.ApplyAndSaveUpgrade(upgradeType);
			}
		}

		private void Update()
		{
			float delta = Time.deltaTime;
			GameController.Update(delta);
		}

		private void FixedUpdate()
		{
			GameController.FixedUpdate(Time.fixedDeltaTime);
		}

		private void OnApplicationQuit()
		{
			GameController.World?.Publish(new GameDataUpdatedEvent());
		}
	}
}