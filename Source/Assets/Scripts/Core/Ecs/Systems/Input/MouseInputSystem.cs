using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	[With(typeof(IsPlayerComp))]
	public sealed class MouseInputSystem : AEntitySetSystem<float>
	{
		public MouseInputSystem(World world) : base(world)
		{
		}

		protected override void Update(float state, in Entity entity)
		{
			Vector3 worldPoint = World.Get<MousePositionComp>().Value;
			ref var playerMouseClick = ref entity.Get<PlayerMouseClickComp>();
			playerMouseClick.Position = worldPoint;

			if (!Input.GetMouseButtonDown(0))
				return;

			playerMouseClick.IsClicked = true;
		}
	}
}