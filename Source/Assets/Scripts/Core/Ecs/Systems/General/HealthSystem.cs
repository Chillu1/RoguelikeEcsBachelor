using System;
using DefaultEcs;
using DefaultEcs.System;

namespace RoguelikeEcs.Core
{
	[With(typeof(HealthComp))]
	public class HealthSystem : CustomEntitySetSystem
	{
		private readonly GameData _gameData;

		public HealthSystem(World world, GameData gameData) : base(world, SystemParameters.UsePlayer)
		{
			_gameData = gameData;
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];
				if (entity.Get<HealthComp>().Current <= 0)
				{
					if (entity.Has<IsPlayerComp>())
					{
						ref var score = ref World.Get<ScoreComp>();
						score.SubmitStreak(World.Get<KillStreakComp>().Streak);
						score.ResetWave(World);

						World.Publish(new FrameStateEndEvent(_gameData, Player));
						World.Publish(new GameDataUpdatedEvent());
						World.Publish(new PauseActionEvent());
						World.Publish(new GameOverEvent());
						continue;
					}

					entity.Set<DestroyComp>();
				}
			}
		}
	}
}