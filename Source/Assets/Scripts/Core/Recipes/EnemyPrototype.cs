using DefaultEcs;

namespace RoguelikeEcs.Core
{
	public sealed class EnemyPrototype
	{
		public EnemyRecipe Recipe { get; }
		public Entity Original { get; }

		public EnemyPrototype(Entity original, EnemyRecipe recipe)
		{
			Original = original;
			Recipe = recipe;
		}
	}
}