using System.Runtime.CompilerServices;

namespace RoguelikeEcs.Core
{
	public struct CritComp
	{
		public float Chance;
		public float Multiplier;

		//private int _n;

		public static CritComp Default = new CritComp
		{
			Chance = 0.1f,
			Multiplier = 2f
		};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Roll()
		{
			//TODO Problem with projectiles, is that they all have their own crit comp. So N is usually 1, unless they pierce.
			//We could have a shared crit comp between all/type X projectiles.
			//bool result = UnityEngine.Random.value < PseudoRandomDistribution.CFromP(Chance * ++_n);
			//if (result)
			//	_n = 0;
			//return result;
			return UnityEngine.Random.value < Chance;
		}
	}
}