namespace RoguelikeEcs.Core
{
	public readonly struct ScoreUpdatedEvent
	{
		public readonly float GoldChange;

		public ScoreUpdatedEvent(float goldChange)
		{
			GoldChange = goldChange;
		}
	}
}