namespace RoguelikeEcs.Core
{
	public struct AttackComp : IRecipeComp
	{
		public float Cooldown;
		public float Timer;
		public bool CanAttack;
	}
}