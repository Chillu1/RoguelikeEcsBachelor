using System.Runtime.CompilerServices;

namespace RoguelikeEcs.Core
{
	public struct KillStreakComp
	{
		private const float KillStreakTime = 5f;

		public int Streak;
		public float TimeLeft;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Refresh()
		{
			Streak++;
			TimeLeft = KillStreakTime;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Reset()
		{
			Streak = 0;
			TimeLeft = 0;
		}
	}
}