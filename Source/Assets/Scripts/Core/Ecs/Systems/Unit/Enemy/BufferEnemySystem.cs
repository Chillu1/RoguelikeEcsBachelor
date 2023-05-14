using System;
using System.Collections.Generic;
using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;
using UnityEngine.UIElements;
using Random = System.Random;

namespace RoguelikeEcs.Core
{
	[With(typeof(IsBufferEnemy))]
	public class BufferEnemySystem : AEntitySetSystem<float>
	{
		private PhysicsScene2D _physicsScene;
		private readonly ContactFilter2D _contactFilter;
		private readonly Collider2D[] _collisionBuffResultsArray;
		Random random = new Random();

		private readonly Sprite _buffSprite;

		public BufferEnemySystem(World world) : base(world)
		{
			_collisionBuffResultsArray = new Collider2D[100];
			_buffSprite = Resources.Load<Sprite>("Textures/Circle");

			_physicsScene = Physics2D.defaultPhysicsScene;
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];
				float radius = entity.Get<RadiusComp>().Value;
				var position = entity.Get<PositionComp>().Value;
				ref var buffer = ref entity.Get<BufferEnemyComp>();

				buffer.Timer += state;
				if (buffer.Timer < buffer.Interval)
					continue;

				var buffType = (BuffType)random.Next(0, 3);

				World.Publish(new AoEAnimationEvent(AnimationType.Circle, position, radius));

				int aoeHits = _physicsScene.OverlapCircle(position, radius, _contactFilter, _collisionBuffResultsArray);
				for (int j = 0; j < aoeHits; j++)
				{
					ref readonly var aoeEnemy = ref _collisionBuffResultsArray[j].gameObject.GetComponent<EnemyReference>().Entity;
					var enemyBuff = aoeEnemy.Get<EnemyBuffComp>();

					if (enemyBuff.Buffs.ContainsKey(buffType))
						continue;

					aoeEnemy.Get<MultiplierComp>().MultipliersList.Add(new MultiplierData()
					{
						StatType = buffType.GetStatType(), Multiplier = 1.5f
					});

					enemyBuff.Buffs[buffType] = new EnemyBuffComp.Timer() { Time = buffer.BuffLength };
				}

				buffer.Timer = 0;
			}
		}
	}
}