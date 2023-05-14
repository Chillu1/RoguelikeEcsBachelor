using System.Collections.Generic;

namespace RoguelikeEcs.Core
{
	public struct EnemyBuffComp
	{
		public Dictionary<BuffType, Timer> Buffs;

		public class Timer
		{
			public float Time;
		}
	}
}