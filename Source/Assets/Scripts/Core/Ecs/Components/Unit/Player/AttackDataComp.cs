using System;
using System.Runtime.CompilerServices;

namespace RoguelikeEcs.Core
{
	public struct AttackDataComp
	{
		public float AoEChance, SniperChance, LifeStealChance;
		public float BurnChance, PoisonChance, ColdChance;

		private int _aoeN, _sniperN, _lifeStealN;
		private int _burnN, _poisonN, _coldN;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Roll(AttackType type)
		{
			return type switch
			{
				AttackType.AoE => PseudoRandomDistribution.Roll(in AoEChance, ref _aoeN),
				AttackType.Sniper => PseudoRandomDistribution.Roll(in SniperChance, ref _sniperN),
				AttackType.LifeSteal => PseudoRandomDistribution.Roll(in LifeStealChance, ref _lifeStealN),
				_ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Roll(EffectType type)
		{
			return type switch
			{
				EffectType.Burn => PseudoRandomDistribution.Roll(in BurnChance, ref _burnN),
				EffectType.Poison => PseudoRandomDistribution.Roll(in PoisonChance, ref _poisonN),
				EffectType.Cold => PseudoRandomDistribution.Roll(in ColdChance, ref _coldN),
				_ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
			};
		}
	}
}