namespace RoguelikeEcs.Core
{
	public struct DifficultyComp
	{
		public DifficultyType Value;
		public bool EnragedWave;

		public bool ToggleEnrage()
		{
			EnragedWave = !EnragedWave;
			return EnragedWave;
		}
	}
}