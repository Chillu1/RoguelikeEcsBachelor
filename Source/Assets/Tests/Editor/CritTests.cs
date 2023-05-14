using NUnit.Framework;
using RoguelikeEcs.Core;
using UnityEngine;

namespace RoguelikeEcs.Tests
{
	public class CritTests : BaseTest
	{
		[Test]
		public void PlayerAlwaysCritUpgradeEnemy()
		{
			//Don't destroy projectile on hit and enemy on death
			DisableSystems(SystemType.Destroy, SystemType.Health);

			World.Get<UpgradesComp>().AlwaysCrit.Enable();

			//Simulated projectile spawn
			var projectile = SpawnProjectile();
			UpdateSystems(); //TODO Check. We need to make an update here, because we added bullet damage multi? _bulletDamageMultiplier?

			//Simulated collision
			ProjectileCollisionSystem.EnemyHit(in projectile, in Enemy);
			UpdateSystems();

			float damage = projectile.Get<DamageComp>().Value;
			float multiplier = projectile.Get<CritComp>().Multiplier * World.Get<UpgradesComp>().CritMultiplier;
			Assert.AreEqual(EnemyHealth - damage * multiplier, Enemy.Get<HealthComp>().Current);
		}
	}
}