using System;
using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	[With(typeof(IsEnemy), typeof(IsDistanceFollowEnemy))]
	public sealed class EnemyFollowDistanceSystem : CustomEntitySetSystem
	{
		public EnemyFollowDistanceSystem(World world) : base(world, SystemParameters.UsePlayer)
		{
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			var playerPosition = Player.Get<PositionComp>().Value;

			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];
				ref readonly var distanceComp = ref entity.Get<DistanceComp>();
				Vector2 direction = Vector2.zero;

				var positionDiff = playerPosition - entity.Get<PositionComp>().Value;
				float distance = positionDiff.magnitude;

				if (distance > distanceComp.Max)
					direction = positionDiff.normalized;
				if (distance < distanceComp.Min)
					direction = -positionDiff.normalized;

				entity.Get<VelocityComp>().Direction = direction;
			}
		}
	}
}