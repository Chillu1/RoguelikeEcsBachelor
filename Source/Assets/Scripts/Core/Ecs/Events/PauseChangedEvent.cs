namespace RoguelikeEcs.Core
{
	public readonly struct PauseChangedEvent
	{
		public readonly bool IsPaused;
		public readonly bool PlayerPaused;

		public PauseChangedEvent(bool isPaused, bool playerPaused)
		{
			IsPaused = isPaused;
			PlayerPaused = playerPaused;
		}
	}
}