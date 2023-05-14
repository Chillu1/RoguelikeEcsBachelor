using DefaultEcs;

namespace RoguelikeEcs.Core
{
	public readonly struct DamageDealtEvent
	{
		public readonly Entity Entity;
		public readonly float TotalDamage;

		public DamageDealtEvent(in Entity entity, float totalDamage)
		{
			Entity = entity;
			TotalDamage = totalDamage;
		}
	}
}