namespace RoguelikeEcs.Core
{
	public struct TimeOutComp
	{
		public float TimeLeft;

		public static readonly TimeOutComp Default = new TimeOutComp { TimeLeft = 7.5f };
	}
}