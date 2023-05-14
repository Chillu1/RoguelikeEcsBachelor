using NUnit.Framework;
using RoguelikeEcs.Core;

namespace RoguelikeEcs.Tests
{
	public class DamageTests : BaseTest
	{
		[Test]
		public void EnemyDamagePlayer()
		{
			Player.Get<DamageEventsComp>().DamageEvents.Add(new DamageEvent()
				{ Source = Enemy, DamageType = Enemy.Get<DamageComp>().DamageType, Damage = Enemy.Get<DamageComp>().Value });

			UpdateSystems();

			Assert.AreEqual(PlayerHealth - EnemyDamage, Player.Get<HealthComp>().Current);
		}
	}
}