using System;
using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	[With(typeof(DoExplodeOnDeathComp))]
	[WhenAdded(typeof(DestroyComp))]
	public sealed class ExplodeOnDeathSystem : CustomEntitySetSystem
	{
		private readonly ContactFilter2D _contactFilter;
		private readonly Collider2D[] _collisionAoEResultsArray;

		private PhysicsScene2D _physicsScene;

		public ExplodeOnDeathSystem(World world) : base(world, SystemParameters.UsePlayer)
		{
			_contactFilter = new ContactFilter2D();
			_collisionAoEResultsArray = new Collider2D[100];
		}

		protected override void PreUpdate(float state)
		{
			_physicsScene = Physics2D.defaultPhysicsScene;
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			var playerPosition = Player.Get<PositionComp>().Value;
			var damageEvents = Player.Get<DamageEventsComp>().DamageEvents;

			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];
				float damage = entity.Get<DamageComp>().Value;
				ref readonly var position = ref entity.Get<PositionComp>();
				ref readonly var radius = ref entity.Get<RadiusComp>();

				//TODO Enemy explosion
				World.Publish(new AoEAnimationEvent(AnimationType.Explosion, position.Value, radius.Value));
				entity.Get<DestroyComp>().Increment();

				if (Vector2.Distance(position.Value, playerPosition) <= radius.Value)
				{
					damageEvents.Add(new DamageEvent { Source = entity, DamageType = DamageType.Explosive, Damage = damage });
				}

				int aoeHits = _physicsScene.OverlapCircle(entity.Get<PositionComp>().Value, entity.Get<RadiusComp>().Value, _contactFilter,
					_collisionAoEResultsArray);
				for (int j = 0; j < aoeHits; j++)
				{
					_collisionAoEResultsArray[j].gameObject.GetComponent<EnemyReference>().Entity.Get<DamageEventsComp>().DamageEvents
						.Add(new DamageEvent() { Source = entity, DamageType = DamageType.Explosive, Damage = damage });
				}
			}
		}
	}
}