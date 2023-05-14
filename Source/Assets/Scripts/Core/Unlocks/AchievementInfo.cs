using DefaultEcs;

namespace RoguelikeEcs.Core
{
	public sealed class AchievementInfo : IUnlockInfo
	{
		public string Name { get; }
		public string DisplayString => Name + " Ach.";
		public string Description { get; private set; }
		public bool IsEndOfWaveUnlock { get; }
		public UnlockAction UnlockAction { get; private set; }

		public AchievementInfo(string name)
		{
			Name = name;
		}

		public AchievementInfo SetDescription(string description)
		{
			Description = description;
			return this;
		}

		public AchievementInfo SetAction(UnlockAction action)
		{
			UnlockAction = action;
			return this;
		}

		public void OnUnlock(GameData gameData, World world, Entity player)
		{
			gameData.CompletedAchievements.Add(Name);
			//TODO Trigger achievement
		}
	}
}