using System;
using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	[With(typeof(IsEnemy), typeof(IsMapEdgeEnemy), typeof(VelocityComp))]
	public sealed class EnemyFollowMapEdgeSystem : CustomEntitySetSystem
	{
		public EnemyFollowMapEdgeSystem(World world) : base(world, SystemParameters.UsePlayer, interval: 0.1f)
		{
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			var playerPosition = Player.Get<PositionComp>().Value;

			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];

				var normalized = (playerPosition - entity.Get<PositionComp>().Value).normalized;
				normalized.x = 0; //Freeze X axis
				entity.Get<VelocityComp>().Direction = normalized;
			}
		}
	}
}