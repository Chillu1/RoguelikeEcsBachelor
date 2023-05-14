namespace RoguelikeEcs.Core
{
	public readonly struct MechanicsHoverOverEvent
	{
		public readonly MechanicType[] MechanicTypes;

		public MechanicsHoverOverEvent(params MechanicType[] mechanicTypes) => MechanicTypes = mechanicTypes;
	}
}