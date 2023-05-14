namespace RoguelikeEcs.Core
{
	public readonly struct WaveCompletedEvent
	{
		public readonly int WaveNumber;

		public WaveCompletedEvent(int waveNumber) => WaveNumber = waveNumber;
	}
}