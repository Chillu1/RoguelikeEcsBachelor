using UnityEngine;

namespace RoguelikeEcs.Core
{
	public enum BuffType
	{
		//None
		HealthBuff,
		SpeedBuff,
		DamageBuff,

		BossHealthBuff
	}

	public static class BuffTypeExtensions
	{
		public static StatType GetStatType(this BuffType buffType)
		{
			switch (buffType)
			{
				case BuffType.HealthBuff:
					return StatType.Health;
				case BuffType.SpeedBuff:
					return StatType.MoveSpeed;
				case BuffType.DamageBuff:
					return StatType.Damage;
				case BuffType.BossHealthBuff:
					return StatType.Health;
				default:
					Debug.LogError($"Unknown buff type: {buffType}");
					return StatType.Health;
			}
		}
	}
}