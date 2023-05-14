namespace RoguelikeEcs.Core
{
	public readonly struct WaveResetEvent
	{
		public readonly ScoreComp Score;

		public WaveResetEvent(ref ScoreComp score) => Score = score;
	}
}