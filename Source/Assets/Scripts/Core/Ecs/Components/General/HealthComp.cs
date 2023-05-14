using System.Runtime.CompilerServices;

namespace RoguelikeEcs.Core
{
	public struct HealthComp
	{
		public float Current;
		public float Max;

		public bool IsMax
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Current >= Max;
		}

		public float Percent
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Current / Max;
		}

		public bool IsDead
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Current <= 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Reset() => Current = Max;
	}
}