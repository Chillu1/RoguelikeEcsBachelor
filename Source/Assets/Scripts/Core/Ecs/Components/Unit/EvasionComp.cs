using System.Runtime.CompilerServices;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	public struct EvasionComp
	{
		public float Chance;

		private int _n;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Roll() => PseudoRandomDistribution.Roll(in Chance, ref _n);
	}
}