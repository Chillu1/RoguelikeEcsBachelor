using System.Runtime.CompilerServices;

namespace RoguelikeEcs.Core
{
	public struct ChargeTimerComp : IRecipeComp
	{
		public float PrepareTime;
		public float Timer;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Reset()
		{
			if (Timer <= 0)
				Timer = PrepareTime;
		}
	}
}