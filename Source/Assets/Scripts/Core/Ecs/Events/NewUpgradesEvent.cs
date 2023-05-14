namespace RoguelikeEcs.Core
{
	public readonly struct NewUpgradesEvent
	{
		public readonly bool IsRefresh;
		public readonly Upgrade[] Upgrades;

		public NewUpgradesEvent(bool isRefresh = false, params Upgrade[] upgrades)
		{
			IsRefresh = isRefresh;
			Upgrades = upgrades;
		}
	}
}