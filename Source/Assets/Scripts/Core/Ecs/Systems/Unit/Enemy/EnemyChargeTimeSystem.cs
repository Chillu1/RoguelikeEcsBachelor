using System;
using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	[With(typeof(IsChargeEnemy))]
	public sealed class EnemyChargeTimeSystem : AEntitySetSystem<float>
	{
		public EnemyChargeTimeSystem(World world) : base(world)
		{
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];

				ref var chargeTimer = ref entity.Get<ChargeTimerComp>();
				chargeTimer.Timer -= state;

				if (chargeTimer.Timer < 0.25f)
				{
					chargeTimer.Reset();
					ref var velocity = ref entity.Get<VelocityComp>();
					velocity.Speed = Mathf.Lerp(velocity.Speed, 480f, 0.05f);
				}
			}
		}
	}
}