using System;
using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	[With(typeof(IsRangedEnemy))]
	public sealed class EnemyRangedAttackSystem : CustomEntitySetSystem
	{
		public EnemyRangedAttackSystem(World world) : base(world, SystemParameters.UsePlayer)
		{
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			var playerPosition = Player.Get<PositionComp>().Value;

			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];
				ref var enemyAttack = ref entity.Get<AttackComp>();

				if (!enemyAttack.CanAttack)
					continue;

				//TODO Possible higher miss chance if player is further away?
				ref readonly var position = ref entity.Get<PositionComp>();
				if (Vector2.Distance(position.Value, playerPosition) > entity.Get<RangeComp>().Value)
					continue;

				enemyAttack.CanAttack = false;

				var attackEntity = World.CreateEntity();
				attackEntity.Set<IsRangedEnemyAttack>();
				attackEntity.Set(new PositionComp { Value = position.Value });
				attackEntity.Set(new RadiusComp { Value = 130f });
				attackEntity.Set(new TimerComp { Value = 0.4f });
				attackEntity.Set(entity.Get<DamageComp>());
				attackEntity.Set(entity.Get<CritComp>());

				float angle = Mathf.Atan2(playerPosition.y - position.Value.y, playerPosition.x - position.Value.x) * Mathf.Rad2Deg;
				Vector2 offset = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * 20f;
				World.Publish(new RangedAttackAnimationEvent(position.Value + offset, 130f, angle));
			}
		}
	}
}