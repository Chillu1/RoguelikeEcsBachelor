using DefaultEcs;

namespace RoguelikeEcs.Core
{
	public static class UnlocksController
	{
		public static void SubscribeUnlocks(World world, UpgradesData upgradesData, AchievementsData achievementsData,
			CharacterData characterData, GameData gameData)
		{
			foreach (var upgrade in upgradesData.GetUpgrades().Values)
			{
				if (!upgrade.IsUnlockable || gameData.UnlockedUpgrades.Contains(upgrade.Type))
					continue;

				upgrade.Subscribe(world);
			}

			foreach (var info in achievementsData.GetAchievementsInfo())
			{
				if (gameData.CompletedAchievements.Contains(info.Name))
					continue;

				info.Subscribe(world);
			}

			foreach (var info in characterData.GetCharactersInfo())
			{
				if (!info.IsUnlockable || gameData.UnlockedCharacters.Contains(info.Type))
					continue;

				info.Subscribe(world);
			}
		}
	}
}