using NUnit.Framework;
using RoguelikeEcs.Core;

namespace RoguelikeEcs.Tests
{
	public sealed class HealthTests : BaseTest
	{
		[Test]
		public void ZeroHealth_EnemyDies()
		{
			Enemy.Get<HealthComp>().Current -= EnemyHealth;

			System.Update(0f);

			Assert.False(Enemy.IsAlive);
		}
	}
}