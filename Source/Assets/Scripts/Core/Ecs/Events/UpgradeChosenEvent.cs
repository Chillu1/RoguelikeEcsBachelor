namespace RoguelikeEcs.Core
{
	public readonly struct UpgradeChosenEvent
	{
		public readonly UpgradeType UpgradeType;

		public UpgradeChosenEvent(UpgradeType upgradeType) => UpgradeType = upgradeType;
	}
}