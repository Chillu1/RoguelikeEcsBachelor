using System.Collections;
using NUnit.Framework;
using RoguelikeEcs.Core;
using UnityEngine;

namespace RoguelikeEcs.Tests
{
	public class ProjectileTests : BaseTest
	{
		//[UnityTest]
		public IEnumerator BulletDamageEnemy()
		{
			//TODO Test doesn't work because the engine doesn't receive any update calls
			var position = new Vector2(30, 0);
			Enemy.Get<PositionComp>().Value = position;
			World.Get<MousePositionComp>().Value = position;

			UpdateSystems(Player.Get<AttackComp>().Cooldown);
			yield return new WaitForFixedUpdate();
			for (int i = 0; i < 120; i++)
			{
				UpdateSystems(0.0167f);
			}

			//Assert.AreEqual(EnemyHealth - PlayerDamage, Enemy.Get<HealthComp>().Current);

			yield return null;
		}

		//[Test]
		public void AoEProjectileDamageEnemies()
		{
		}
	}
}