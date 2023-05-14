using System;
using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	[With(typeof(IsPlayerComp), typeof(RendererComp))]
	public sealed class PlayerDirectionRenderingSystem : CustomEntitySetSystem
	{
		public PlayerDirectionRenderingSystem(World world) :
			base(world, interval: 0.1f)
		{
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];
				//Dont change rotation if player is not moving
				if (entity.Get<VelocityComp>().Direction.x == 0)
					continue;

				float yRotation = entity.Get<VelocityComp>().Direction.x < 0 ? 180 : 0;
				entity.Get<RendererComp>().Child.transform.rotation = Quaternion.Euler(0, yRotation, 0);
			}
		}
	}
}