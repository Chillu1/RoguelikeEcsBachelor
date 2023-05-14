using NUnit.Framework;
using RoguelikeEcs.Core;

namespace RoguelikeEcs.Tests
{
	public class EvasionTests : BaseTest
	{
		[Test]
		public void PlayerEvadesEnemyAttack()
		{
			DisableSystems(SystemType.Health, SystemType.Wave);

			Player.Get<EvasionComp>().Chance = 1f;

			for (int i = 0; i < 100; i++)
				UpdateSystems(Enemy.Get<AttackComp>().Cooldown);

			Assert.AreEqual(PlayerHealth, Player.Get<HealthComp>().Current);
		}

		[Test]
		public void EnemyEvadesPlayerProjectile()
		{
			Enemy.Get<EvasionComp>().Chance = 1f;

			for (int i = 0; i < 10; i++)
			{
				//Simulated projectile spawn
				var projectile = SpawnProjectile();

				//Simulated collision
				ProjectileCollisionSystem.EnemyHit(in projectile, in Enemy);
			}

			UpdateSystems();

			Assert.AreEqual(EnemyHealth, Enemy.Get<HealthComp>().Current);
		}

		[Test]
		public void PlayerEvasionCursedEnemyAttack()
		{
			DisableSystems(SystemType.Health, SystemType.Wave);

			Player.Get<EvasionComp>().Chance = 1f;
			Player.Set<DoIgnoreEvasion>();

			UpdateSystems(Enemy.Get<AttackComp>().Cooldown);

			Assert.AreEqual(PlayerHealth - EnemyDamage, Player.Get<HealthComp>().Current);
		}
	}
}