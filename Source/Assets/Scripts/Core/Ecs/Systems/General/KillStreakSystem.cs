using DefaultEcs;
using DefaultEcs.System;

namespace RoguelikeEcs.Core
{
	public sealed class KillStreakSystem : ISystem<float>
	{
		public bool IsEnabled { set; get; } = true;

		private readonly World _world;

		public KillStreakSystem(World world)
		{
			_world = world;
		}

		public void Update(float state)
		{
			if (!IsEnabled)
				return;

			ref var killStreak = ref _world.Get<KillStreakComp>();
			if (killStreak.TimeLeft <= 0)
				return;

			killStreak.TimeLeft -= state;

			if (killStreak.TimeLeft <= 0)
			{
				_world.Get<ScoreComp>().SubmitStreak(killStreak.Streak);
				killStreak.Reset();
			}
		}

		public void Dispose()
		{
		}
	}
}