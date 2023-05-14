using UnityEngine;

namespace RoguelikeEcs.Core
{
	public struct WandComp
	{
		public SpriteRenderer Renderer;
		public GameObject Child;
		public Animator Animator;

		public Vector2 TipPoint;
	}
}