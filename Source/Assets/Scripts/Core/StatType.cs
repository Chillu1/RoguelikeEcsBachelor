namespace RoguelikeEcs.Core
{
	public enum StatType
	{
		//None,
		Health,
		Damage,
		AttackSpeed,
		MoveSpeed,
	}

	public static class StatTypeExtensions
	{
		public static bool IsPoolStat(this StatType statType)
		{
			return statType == StatType.Health;
		}
	}
}