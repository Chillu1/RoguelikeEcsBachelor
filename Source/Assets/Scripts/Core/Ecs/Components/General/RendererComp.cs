using UnityEngine;

namespace RoguelikeEcs.Core
{
	public struct RendererComp : IRecipeComp
	{
		public SpriteRenderer Value;
		public GameObject Child;
		public Color OriginalColor; //TODO Move Me
		public bool HasOriginalColor; //TODO Move me
		public float WasHitTimer;
	}
}