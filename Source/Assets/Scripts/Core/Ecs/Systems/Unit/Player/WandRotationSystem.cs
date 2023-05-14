using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	[With(typeof(IsPlayerComp))]
	public sealed class WandRotationSystem : AEntitySetSystem<float>
	{
		public WandRotationSystem(World world) : base(world)
		{
		}

		protected override void Update(float state, in Entity entity)
		{
			ref var wand = ref entity.Get<WandComp>();
			var worldPoint = World.Get<MousePositionComp>().Value;
			var playerPosition = entity.Get<PositionComp>().Value;

			var dir = worldPoint - playerPosition;
			float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

			var wandTransform = wand.Renderer.transform;
			var wandLocalPosition = wandTransform.localPosition;
			wandLocalPosition.x = Mathf.Cos(angle * Mathf.Deg2Rad) * 7;
			wandLocalPosition.y = Mathf.Sin(angle * Mathf.Deg2Rad) * 7;

			wandTransform.localPosition = Vector3.Lerp(wandTransform.localPosition, wandLocalPosition, state * 10f);
			wandTransform.rotation =
				Quaternion.Lerp(wandTransform.rotation, Quaternion.AngleAxis(angle - 90, Vector3.forward), state * 10f);

			wand.TipPoint = playerPosition + dir.normalized * 28;
		}
	}
}