using NUnit.Framework;
using RoguelikeEcs.Core;
using UnityEngine;

namespace RoguelikeEcs.Tests
{
	public class AttackTests : BaseTest
	{
		[Test]
		public void PlayerAttackEnemy()
		{
			//TODO This test fails sometimes on 1st run. Maybe because first test? Seems more that it fails when EnemyAttackPlayer is ran first?
			//Don't destroy projectile on hit and enemy on death
			DisableSystems(SystemType.Destroy, SystemType.Health);

			//Simulated projectile spawn
			var projectile = SpawnProjectile();
			UpdateSystems(); //TODO Check. We need to make an update here, because we added bullet damage multi? _bulletDamageMultiplier?

			//Simulated collision
			ProjectileCollisionSystem.EnemyHit(in projectile, in Enemy);
			UpdateSystems();

			float damage = projectile.Get<DamageComp>().Value;
			Assert.AreEqual(EnemyHealth - damage, Enemy.Get<HealthComp>().Current);
		}

		[Test]
		public void EnemyAttackPlayer()
		{
			//TODO This is weird. This test erroring makes it so other tests also fail. Bad test isolation.
			DisableSystems(SystemType.Health | SystemType.Wave);

			Player.Get<PositionComp>().Value = Enemy.Get<PositionComp>().Value = Vector2.zero;

			UpdateSystems(Enemy.Get<AttackComp>().Cooldown);

			Assert.AreEqual(PlayerHealth - EnemyDamage, Player.Get<HealthComp>().Current);
		}
	}
}