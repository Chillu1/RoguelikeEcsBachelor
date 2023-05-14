using System;

namespace RoguelikeEcs.Core
{
	public struct ResistancesComp
	{
		public float PhysicalPercent;
		public float FirePercent;
		public float ColdPercent;
		public float BleedPercent;
		public float ExplosivePercent;

		public float PhysicalValue;
		public float FireValue;
		public float ColdValue;
		public float BleedValue;
		public float ExplosiveValue;

		public static ResistancesComp Default => new ResistancesComp
		{
			PhysicalPercent = 1,
			FirePercent = 1,
			ColdPercent = 1,
			BleedPercent = 1,
			ExplosivePercent = 1
		};

		public void AddValue(DamageType type, float value)
		{
			switch (type)
			{
				case DamageType.Physical:
					PhysicalValue += value;
					PhysicalPercent = 1f - Curves.DamageResistance.Evaluate(PhysicalValue);
					break;
				case DamageType.Fire:
					FireValue += value;
					FirePercent = 1f - Curves.DamageResistance.Evaluate(FireValue);
					break;
				case DamageType.Cold:
					ColdValue += value;
					ColdPercent = 1f - Curves.DamageResistance.Evaluate(ColdValue);
					break;
				case DamageType.Bleed:
					BleedValue += value;
					BleedPercent = 1f - Curves.DamageResistance.Evaluate(BleedValue);
					break;
				case DamageType.Explosive:
					ExplosiveValue += value;
					ExplosivePercent = 1f - Curves.DamageResistance.Evaluate(ExplosiveValue);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}
		}

		public readonly float GetPercent(DamageType type)
		{
			switch (type)
			{
				case DamageType.Physical:
					return PhysicalPercent;
				case DamageType.Fire:
					return FirePercent;
				case DamageType.Cold:
					return ColdPercent;
				case DamageType.Bleed:
					return BleedPercent;
				case DamageType.Explosive:
					return ExplosivePercent;
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}
		}
	}
}