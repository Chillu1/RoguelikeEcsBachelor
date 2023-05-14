using System;
using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	[With(typeof(IsEnemy))]
	[Without(typeof(FlashEffectComp))]
	public sealed class HealthDisplaySystem : CustomEntitySetSystem
	{
		private const float MinBrightness = 0.35f;

		public HealthDisplaySystem(World world) : base(world, interval: 0.02f)
		{
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];
				ref readonly var health = ref entity.Get<HealthComp>();
				ref var renderer = ref entity.Get<RendererComp>();

				if (health.IsMax)
				{
					if (!renderer.HasOriginalColor)
					{
						renderer.Value.color = renderer.OriginalColor;
						renderer.HasOriginalColor = true;
					}

					continue;
				}

				Color.RGBToHSV(renderer.OriginalColor, out float hue, out float saturation, out float brightness);
				brightness = Mathf.Lerp(MinBrightness, brightness, health.Percent);
				renderer.Value.color = Color.HSVToRGB(hue, saturation, brightness);

				renderer.HasOriginalColor = false;
			}
		}
	}
}