using System.Runtime.CompilerServices;

namespace RoguelikeEcs.Core
{
	public enum UpgradeState
	{
		OffBuffer2 = -3,
		OffBuffer1,

		/// <summary>
		///		Never
		/// </summary>
		Off,

		Neutral = 0,

		/// <summary>
		///		Always
		/// </summary>
		On,
		OnBuffer1,
		OnBuffer2,
	}

	public static class UpgradeStateExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsOn(this UpgradeState state)
		{
			return state >= UpgradeState.On;
		}

		public static bool IsOff(this UpgradeState state)
		{
			return state <= UpgradeState.Off;
		}

		public static void Enable(ref this UpgradeState state)
		{
			if (state > UpgradeState.OnBuffer2)
				return;

			state++;
		}

		public static void Disable(ref this UpgradeState state)
		{
			if (state < UpgradeState.OffBuffer2)
				return;

			state--;
		}
	}
}