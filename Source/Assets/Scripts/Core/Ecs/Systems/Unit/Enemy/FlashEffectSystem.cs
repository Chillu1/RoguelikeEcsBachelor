using System;
using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	[With(typeof(FlashEffectComp))]
	public sealed class FlashEffectSystem : AEntitySetSystem<float>
	{
		private float _timer;

		public FlashEffectSystem(World world) : base(world)
		{
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			_timer += state;

			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];
				ref readonly var renderer = ref entity.Get<RendererComp>();
				ref readonly var flashEffect = ref entity.Get<FlashEffectComp>();
				renderer.Value.color = Color.Lerp(renderer.OriginalColor, flashEffect.Value,
					Mathf.PingPong(_timer * flashEffect.IntervalMultiplier, 1f));
			}
		}
	}
}