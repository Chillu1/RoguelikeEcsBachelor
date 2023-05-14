using NUnit.Framework;
using RoguelikeEcs.Core;

namespace RoguelikeEcs.Tests
{
	public class EffectTests : BaseTest
	{
		[Test]
		public void ColdAttack_SlowedEnemy()
		{
			float speedMultiplier = Enemy.Get<MultiplierComp>().BaseMultipliers[(int)StatType.MoveSpeed];
			Player.Get<AttackDataComp>().ColdChance = 1f;

			var projectile = SpawnProjectile();
			float slowMultiplier = projectile.Get<ColdDataComp>().SlowMultiplier;

			ProjectileCollisionSystem.EnemyHit(in projectile, in Enemy);
			UpdateSystems();

			Assert.AreEqual(speedMultiplier * slowMultiplier, Enemy.Get<MultiplierComp>().BaseMultipliers[(int)StatType.MoveSpeed]);
		}
	}
}