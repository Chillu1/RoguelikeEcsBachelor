using DefaultEcs;

namespace RoguelikeEcs.Core
{
	public readonly struct CheckUnlocksEvent
	{
		public readonly GameData GameData;
		public readonly ScoreComp Score;
		public readonly Entity Player;

		public CheckUnlocksEvent(GameData gameData, ScoreComp score, Entity player)
		{
			GameData = gameData;
			Score = score;
			Player = player;
		}
	}
}