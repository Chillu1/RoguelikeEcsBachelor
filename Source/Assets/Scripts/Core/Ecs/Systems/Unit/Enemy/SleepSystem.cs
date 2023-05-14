using System;
using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	[With(typeof(IsSleeping))]
	public sealed class SleepSystem : CustomEntitySetSystem
	{
		public SleepSystem(World world) : base(world, SystemParameters.UsePlayer | SystemParameters.UseBuffer, interval: 0.1f)
		{
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			var playerPosition = Player.Get<PositionComp>().Value;

			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];

				if (!entity.Get<HealthComp>().IsMax)
				{
					entity.Remove<IsSleeping>();
					continue;
				}

				//TODO Scale with enemy scale
				if (Vector2.Distance(playerPosition, entity.Get<PositionComp>().Value) < 25f)
					entity.Remove<IsSleeping>();
			}
		}
	}
}