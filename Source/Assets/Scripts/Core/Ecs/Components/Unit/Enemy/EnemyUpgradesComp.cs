namespace RoguelikeEcs.Core
{
	public struct EnemyUpgradesComp
	{
		public UpgradeState AlwaysCrit;

		public float CritMultiplier;

		public static EnemyUpgradesComp Default => new EnemyUpgradesComp
		{
			CritMultiplier = 1f
		};
	}
}