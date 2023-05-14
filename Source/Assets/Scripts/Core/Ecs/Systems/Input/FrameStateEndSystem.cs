using DefaultEcs;
using DefaultEcs.System;

namespace RoguelikeEcs.Core
{
	[With(typeof(IsPlayerComp))]
	public sealed class FrameStateEndSystem : CustomEntitySetSystem
	{
		private readonly GameData _gameData;

		public FrameStateEndSystem(World world, GameData gameData) : base(world, interval: 1f)
		{
			_gameData = gameData;
		}

		protected override void Update(float state, in Entity entity)
		{
			World.Publish(new FrameStateEndEvent(_gameData, entity));
		}
	}
}