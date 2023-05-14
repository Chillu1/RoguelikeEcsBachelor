using System;
using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	[With(typeof(IsEnemy), typeof(HasCorpseExplosion))]
	public sealed class CurseSystem : AEntitySetSystem<float>
	{
		private PhysicsScene2D _physicsScene;
		private readonly ContactFilter2D _contactFilter;
		private readonly Collider2D[] _collisions;

		public CurseSystem(World world) : base(world)
		{
			_physicsScene = Physics2D.defaultPhysicsScene;
			_contactFilter = new ContactFilter2D();
			_collisions = new Collider2D[50];
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];

				if (entity.Get<HealthComp>().Current <= 0)
				{
					//Corpse Explosion
					//Deal damage to all enemies in a radius
					int hits = _physicsScene.OverlapCircle(entity.Get<PositionComp>().Value, 10f, _contactFilter, _collisions);
					for (int j = 0; j < hits; j++)
					{
						_collisions[j].GetComponent<EnemyReference>().Entity.Get<HealthComp>().Current -= 10;
					}
				}
			}
		}
	}
}