using System.Collections;
using NUnit.Framework;
using RoguelikeEcs.Core;
using UnityEngine;
using UnityEngine.TestTools;

namespace RoguelikeEcs.Tests
{
	public class EnemyMovementTests : BaseTest
	{
		[UnityTest]
		public IEnumerator EnemyMoveToPlayer()
		{
			Player.Get<HealthComp>().Current = int.MaxValue;
			Enemy.Get<HealthComp>().Current = int.MaxValue;
			var startPosition = new Vector2(20, 0);
			Enemy.Get<PositionComp>().Value = startPosition;
			Enemy.Get<GameObjectComp>().Value.transform.position = startPosition;

			Assert.AreNotEqual(Player.Get<PositionComp>().Value, Enemy.Get<PositionComp>().Value);

			UpdateSystems(DeltaTime);
			yield return null;

			Assert.AreNotEqual(startPosition, Enemy.Get<PositionComp>().Value);

			for (int i = 0; i < 15; i++)
			{
				UpdateSystems(DeltaTime);
				yield return null;
			}

			Assert.True(Vector2.Distance(Player.Get<PositionComp>().Value, Enemy.Get<PositionComp>().Value) < 5);
		}
	}
}