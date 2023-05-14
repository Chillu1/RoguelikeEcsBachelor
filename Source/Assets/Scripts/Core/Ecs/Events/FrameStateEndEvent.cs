using DefaultEcs;

namespace RoguelikeEcs.Core
{
	/// <summary>
	///		Achievements and unlocks event
	/// </summary>
	public readonly struct FrameStateEndEvent
	{
		public readonly GameData GameData;
		public readonly Entity Player;

		public FrameStateEndEvent(GameData gameData, Entity player)
		{
			GameData = gameData;
			Player = player;
		}
	}
}