using System;
using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	[With(typeof(IsRangedEnemyAttack))]
	public sealed class EnemyRangedDelayedAttackSystem : CustomEntitySetSystem
	{
		public EnemyRangedDelayedAttackSystem(World world) : base(world, SystemParameters.UsePlayer | SystemParameters.UseBuffer)
		{
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			var playerPosition = Player.Get<PositionComp>().Value;
			bool doIgnoreEvasion = Player.Has<DoIgnoreEvasion>();
			ref var playerEvasion = ref Player.Get<EvasionComp>();
			var damageEvents = Player.Get<DamageEventsComp>().DamageEvents;

			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];

				ref var timer = ref entity.Get<TimerComp>();
				timer.Value -= state;
				if (timer.Value > 0)
					continue;

				if (Vector2.Distance(entity.Get<PositionComp>().Value, playerPosition) < entity.Get<RadiusComp>().Value
				    && (doIgnoreEvasion || !playerEvasion.Roll()))
				{
					ref readonly var damage = ref entity.Get<DamageComp>();
					damageEvents.Add(new DamageEvent() { Source = entity, DamageType = damage.DamageType, Damage = damage.Value });
				}

				entity.Set<DestroyComp>();
			}
		}
	}
}