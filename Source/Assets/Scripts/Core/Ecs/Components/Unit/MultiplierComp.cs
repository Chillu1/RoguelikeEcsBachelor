using System.Collections.Generic;

namespace RoguelikeEcs.Core
{
	public struct MultiplierComp
	{
		public float[] BaseMultipliers;
		public List<MultiplierData> MultipliersList;
	}

	public struct MultiplierData
	{
		public StatType StatType;
		public float Multiplier;
	}
}