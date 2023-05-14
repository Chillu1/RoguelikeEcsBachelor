using System.Runtime.CompilerServices;

namespace RoguelikeEcs.Core
{
	public struct HomingDataComp
	{
		public float Chance;

		private int _n;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Roll() => PseudoRandomDistribution.Roll(in Chance, ref _n);
	}
}