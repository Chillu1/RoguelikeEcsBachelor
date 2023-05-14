using System;
using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	[With(typeof(IsEnemy), typeof(RendererComp))]
	public sealed class EnemyAttackedFlashSystem : CustomEntitySetSystem
	{
		public EnemyAttackedFlashSystem(World world) : base(world)
		{
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];
				ref var renderer = ref entity.Get<RendererComp>();
				if (renderer.WasHitTimer <= 0f)
					continue;

				renderer.WasHitTimer -= state;

				renderer.Value.color = Color.Lerp(renderer.Value.color, Color.red, Mathf.PingPong(renderer.WasHitTimer, 1f));
			}
		}
	}
}