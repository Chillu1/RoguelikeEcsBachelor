namespace RoguelikeEcs.Core
{
	public readonly struct GetNewUpgradesEvent
	{
		public readonly bool IsLoad;

		public GetNewUpgradesEvent(bool isLoad) => IsLoad = isLoad;
	}
}