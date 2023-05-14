namespace RoguelikeEcs.Core
{
	public readonly struct UpgradeUnlockedEvent
	{
		public readonly UpgradeType Type;

		public UpgradeUnlockedEvent(UpgradeType upgradeType) => Type = upgradeType;
	}
}