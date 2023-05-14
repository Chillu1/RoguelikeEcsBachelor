namespace RoguelikeEcs.Core
{
	public sealed class UpgradeInfo
	{
		public Upgrade Upgrade { get; }

		//public RarityType Rarity { get; }
		public int Weight { get; }

		public UpgradeInfo(Upgrade upgrade, int weight)
		{
			Upgrade = upgrade;
			Weight = weight;
		}
	}
}