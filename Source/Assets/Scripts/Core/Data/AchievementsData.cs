using System.Collections.Generic;

namespace RoguelikeEcs.Core
{
	public sealed class AchievementsData
	{
		private readonly List<AchievementInfo> _achievements;

		public AchievementsData()
		{
			_achievements = new List<AchievementInfo>();
			SetupAchievements();
		}

		public IReadOnlyList<AchievementInfo> GetAchievementsInfo() => _achievements;

		private void SetupAchievements()
		{
			AchievementInfo Add(string name)
			{
				var achievementInfo = new AchievementInfo(name);
				_achievements.Add(achievementInfo);
				return achievementInfo;
			}


			Add("First Blood")
				.SetDescription("Kill your first enemy")
				.SetAction((gameData, world, player, score) => gameData.TotalKills >= 1);

			Add("First Boss")
				.SetDescription("Kill your first boss")
				.SetAction((gameData, world, player, score) => gameData.TotalBossKills >= 1);

			Add("Player Instakill")
				.SetDescription("Die from a single enemy hit while on full health")
				.SetAction((gameData, world, player, score) =>
				{
					ref readonly var health = ref player.Get<HealthComp>();

					return health.IsDead && player.Get<PreviousHealthComp>().Value >= health.Max;
				});

			// Add("First Win")
			// 	.SetDescription("Win your first game");
		}
	}
}