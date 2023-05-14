namespace RoguelikeEcs.Core
{
	public readonly struct UnlockEvent
	{
		public readonly IUnlockInfo Info;

		public UnlockEvent(IUnlockInfo info) => Info = info;
	}
}