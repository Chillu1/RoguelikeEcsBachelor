using System.Collections.Generic;
using DefaultEcs;

namespace RoguelikeEcs.Core
{
	public struct DamageEventsComp
	{
		public List<DamageEvent> DamageEvents;
	}

	public struct DamageEvent
	{
		public Entity Source;
		public DamageType DamageType;
		public float Damage;
	}
}