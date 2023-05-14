using UnityEngine;

namespace RoguelikeEcs.Core
{
	public struct FlashEffectComp : IRecipeComp
	{
		public Color Value;
		public float IntervalMultiplier;
	}
}