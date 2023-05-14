using System;
using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	[With(typeof(IsMeleeEnemy))]
	public class EnemyMeleeAttackSystem : CustomEntitySetSystem
	{
		private const float DistanceToHit = 12f;

		public EnemyMeleeAttackSystem(World world) : base(world, SystemParameters.UsePlayer)
		{
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			var playerPosition = Player.Get<PositionComp>().Value;
			var damageEvents = Player.Get<DamageEventsComp>().DamageEvents;
			ref var playerEvasion = ref Player.Get<EvasionComp>();
			bool doIgnoreEvasion = Player.Has<DoIgnoreEvasion>();

			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];
				ref var enemyAttack = ref entity.Get<AttackComp>();

				if (!enemyAttack.CanAttack)
					continue;

				//TODO Possible higher miss chance if player is further away?
				if (Vector2.Distance(entity.Get<PositionComp>().Value, playerPosition) >
				    DistanceToHit + entity.Get<ScaleComp>().Magnitude * 1.7f)
					continue;

				enemyAttack.CanAttack = false;

				if (!doIgnoreEvasion && playerEvasion.Roll())
					continue;

				ref readonly var damage = ref entity.Get<DamageComp>();
				damageEvents.Add(new DamageEvent() { Source = entity, DamageType = damage.DamageType, Damage = damage.Value });
			}
		}
	}
}